namespace Manager.Domain.Services
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IMapper _comprobanteMapper;
        private readonly IComprobanteRepository _comprobanteRepository;
        private readonly IZipReader _zipReader;
        private readonly IXmlInvoiceParserService _xmlInvoiceParserService;

        private readonly ICpeService _cpeService;
        private readonly IConfiguracionGlobalService _configuracionGlobalService;


        private readonly ILogger<ComprobanteService> _logger;

        public ComprobanteService(IComprobanteRepository comprobanteRepository, IZipReader zipReader, IXmlInvoiceParserService xmlInvoiceParserService, ICpeService cpeService, IConfiguracionGlobalService configuracionGlobalService, IMapper comprobanteMapper, ILogger<ComprobanteService> logger)
        {
            _comprobanteRepository = comprobanteRepository;
            _zipReader = zipReader;
            _xmlInvoiceParserService = xmlInvoiceParserService;
            _cpeService = cpeService;
            _configuracionGlobalService = configuracionGlobalService;
            _comprobanteMapper = comprobanteMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync(Guid perTributarioId, string search)
        {
            var result = await _comprobanteRepository.GetAsync(predicate: z => z.PerTributarioId == perTributarioId && (z.NombreProveedor.Contains(search) || z.NumeroDocIdentidad.Contains(search) || z.Serie.Contains(search) || z.Numero.Contains(search)));

            return result
                .Select(x => _comprobanteMapper.Map<ComprobanteResponse>(x));
        }

        public async Task<BaseResponseGeneric<ICollection<Comprobante_GlosaResponse>>> ImportarGlosaAsync(
            Guid perTributarioId,
            string token,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponseGeneric<ICollection<Comprobante_GlosaResponse>>();
            var results = new ConcurrentBag<Comprobante_GlosaResponse>();
            int maxConcurrent = 3; // 🔹 Reduce concurrencia para evitar saturar red o CPU

            try
            {
                var comprobantes = await _comprobanteRepository.GetAsync(
                    predicate: z => z.PerTributarioId == perTributarioId && !z.TieneGlosa);

                if (comprobantes == null || !comprobantes.Any())
                {
                    return new BaseResponseGeneric<ICollection<Comprobante_GlosaResponse>>
                    {
                        Success = false,
                        Message = "No se encontraron comprobantes para procesar.",
                        Data = new List<Comprobante_GlosaResponse>(),
                        StatusCode = 404
                    };
                }

                _logger.LogInformation("Iniciando importación de glosas para {Count} comprobantes", comprobantes.Count);

                await Parallel.ForEachAsync(comprobantes,
                    new ParallelOptions { MaxDegreeOfParallelism = maxConcurrent, CancellationToken = cancellationToken },
                    async (cpe, ct) =>
                    {
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        cts.CancelAfter(TimeSpan.FromMinutes(2)); // ⏱ timeout por comprobante

                        try
                        {
                            var zipBytes = await EjecutarConResilienciaAsync(
                                () => _cpeService.DescargarZipAsync(token, new DescargarZipRequest
                                {
                                    RucEmisor = cpe.NumeroDocIdentidad,
                                    TipoComprobante = cpe.TipoComprobante,
                                    Numero = Convert.ToInt32(cpe.Numero),
                                    Serie = cpe.Serie,
                                    Tipo = "02"
                                }));

                            _logger.LogInformation("Descarga de ZIP completada para comprobante {Serie}-{Numero}", cpe.Serie, cpe.Numero);
                            _logger.LogInformation("Resultado de descarga: {Success}, Mensaje: {Message}", zipBytes.Success, zipBytes.Message);

                            if (zipBytes.Success && zipBytes.Data?.Archivo != null)
                            {
                                var resultado = await ProcesarAsync(zipBytes.Data.Archivo, cpe, cts.Token);

                                results.Add(new Comprobante_GlosaResponse
                                {
                                    Id = cpe.Id,
                                    Serie = cpe.Serie,
                                    Numero = cpe.Numero,
                                    Exito = resultado.Success,
                                    Mensaje = resultado.Message ?? "Procesado correctamente."
                                });
                            }
                            else
                            {
                                results.Add(new Comprobante_GlosaResponse
                                {
                                    Id = cpe.Id,
                                    Serie = cpe.Serie,
                                    Numero = cpe.Numero,
                                    Exito = false,
                                    Mensaje = $"Error al descargar ZIP: {zipBytes.Message ?? "Error desconocido"}"
                                });
                            }
                        }
                        catch (OperationCanceledException)
                        {
                            results.Add(new Comprobante_GlosaResponse
                            {
                                Id = cpe.Id,
                                Serie = cpe.Serie,
                                Numero = cpe.Numero,
                                Exito = false,
                                Mensaje = "Tiempo de espera excedido (timeout)."
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error procesando comprobante {Serie}-{Numero}", cpe.Serie, cpe.Numero);

                            results.Add(new Comprobante_GlosaResponse
                            {
                                Id = cpe.Id,
                                Serie = cpe.Serie,
                                Numero = cpe.Numero,
                                Exito = false,
                                Mensaje = $"Error inesperado: {ex.Message}"
                            });
                        }
                    });

                var total = results.Count;
                var exitosos = results.Count(r => r.Exito);
                var fallidos = total - exitosos;

                response.Success = true;
                response.Message = $"Importación completada. Total: {total}, Éxitos: {exitosos}, Fallos: {fallidos}.";
                response.Data = results.ToList();
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error global al importar glosas.");
                response.Success = false;
                response.Message = $"Error inesperado: {ex.Message}";
                response.ErrorCode = "GLOSA_IMPORT_ERROR";
                response.StatusCode = 500;
                response.Data = new List<Comprobante_GlosaResponse>();
            }

            return response;
        }

        public async Task<ComprobanteResponse> EditComprobanteAsync(EditComprobanteRequest request)
        {
            var existingRecord = await _comprobanteRepository.GetAsync(request.Id);

            if (existingRecord == null)
                throw new ArgumentException($"Comprobante con ID {request.Id} no existe.");

            // Solo actualizas los campos que cambian
            existingRecord.Glosa = request.Glosa;
            existingRecord.TieneGlosa = request.TieneGlosa;

            // Actualizas directamente el registro existente
            _comprobanteRepository.UpdateAsync(existingRecord);
            await _comprobanteRepository.UnitOfWork.SaveChangesAsync();

            return _comprobanteMapper.Map<ComprobanteResponse>(existingRecord);
        }

        private async Task<BaseResponseGeneric<Comprobante_GlosaResponse>> ProcesarAsync(
            byte[] zipFile, 
            Comprobante existingRecord, 
            CancellationToken cancellationToken)
        {
            var response = new BaseResponseGeneric<Comprobante_GlosaResponse>();

            try
            {
                // 1️⃣ Leer ZIP
                var xmlContent = _zipReader.ExtractXmlFromZip(zipFile);

                // 2️⃣ Parsear XML
                var invoice = _xmlInvoiceParserService.ParseInvoice(xmlContent);
                var config = await _configuracionGlobalService.GetConfiguracionGlobalFirstOrDefaultAsync();

                // 3️⃣ Construir glosa
                var max = config.MaxCaracteresGlosa > 0 ? config.MaxCaracteresGlosa : 100;
                var glosa = string.Join("; ", invoice.InvoiceLines.Select(l =>
                {
                    var desc = l.Description ?? string.Empty;
                    return desc.Length > max ? desc.Substring(0, max) + "..." : desc;
                }));

                // 4️⃣ Guardar en BD
                existingRecord.Glosa = glosa;
                existingRecord.TieneGlosa = true;

                await _comprobanteRepository.UpdateAsync(existingRecord);
                await _comprobanteRepository.UnitOfWork.SaveChangesAsync();

                // 5️⃣ Construir respuesta exitosa
                response.Success = true;
                response.Message = "Glosa procesada correctamente.";
                response.StatusCode = 200;
                response.Data = new Comprobante_GlosaResponse
                {
                    Id = existingRecord.Id,
                    Serie = existingRecord.Serie,
                    Numero = existingRecord.Numero,
                    Glosa = glosa,
                    NombreArchivo = $"{existingRecord.NumeroDocIdentidad}-{existingRecord.TipoComprobante}-{existingRecord.Serie}-{existingRecord.Numero}"
                };
            }
            catch (Exception ex)
            {
                // 🛑 Capturamos cualquier error
                response.Success = false;
                response.Message = $"Error al procesar glosa: {ex.Message}";
                response.ErrorCode = "GLOSA_PROCESS_ERROR";
                response.StatusCode = 500;
                response.Data = null;

                _logger.LogError(ex, "Error procesando comprobante {Serie}-{Numero}", existingRecord.Serie, existingRecord.Numero);
            }

            return response;
        }

        private async Task<BaseResponseGeneric<T>> EjecutarConResilienciaAsync<T>(
            Func<Task<BaseResponseGeneric<T>>> action,
            int maxRetries = 1,
            int initialDelayMs = 2000)
        {
            var response = new BaseResponseGeneric<T>();
            int attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    attempt++;
                    var result = await action();

                    if (result.Success)
                        return result;

                    _logger.LogWarning("Intento {Attempt}/{Max} falló: {Message}", attempt, maxRetries, result.Message);

                    // Espera incremental
                    await Task.Delay(initialDelayMs * attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ejecutando intento {Attempt}/{Max}.", attempt, maxRetries);
                    await Task.Delay(initialDelayMs * attempt);
                }
            }

            response.Success = false;
            response.Message = $"No se pudo completar la operación después de {maxRetries} intentos.";
            response.ErrorCode = "RETRY_FAILED";
            response.StatusCode = 500;
            response.Data = default;

            return response;
        }

    }
}
