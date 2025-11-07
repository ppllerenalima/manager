using System.Text.RegularExpressions;

namespace Manager.Domain.Services
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IMapper _comprobanteMapper;
        private readonly IComprobanteRepository _comprobanteRepository;
        private readonly IZipReader _zipReader;
        private readonly IPdfReader _pdfReader;
        private readonly IXmlInvoiceParserService _xmlInvoiceParserService;

        private readonly ICpeService _cpeService;
        private readonly IConfiguracionGlobalService _configuracionGlobalService;


        private readonly ILogger<ComprobanteService> _logger;

        public ComprobanteService(IComprobanteRepository comprobanteRepository, IZipReader zipReader, IPdfReader pdfReader, IXmlInvoiceParserService xmlInvoiceParserService, ICpeService cpeService, IConfiguracionGlobalService configuracionGlobalService, IMapper comprobanteMapper, ILogger<ComprobanteService> logger)
        {
            _comprobanteRepository = comprobanteRepository;
            _zipReader = zipReader;
            _pdfReader = pdfReader;

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
                                }));

                            _logger.LogInformation("Descarga de ZIP completada para comprobante {Serie}-{Numero}", cpe.Serie, cpe.Numero);
                            _logger.LogInformation("Resultado de descarga: {Success}, Mensaje: {Message}", zipBytes.Success, zipBytes.Message);

                            if (zipBytes.Success && zipBytes.Data?.Archivo != null)
                            {
                                var resultado = await ProcesarAsync(zipBytes.Data.Tipo, zipBytes.Data.Archivo, cpe, cts.Token);

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
            string tipo,
            byte[] zipFile,
            Comprobante existingRecord,
            CancellationToken cancellationToken)
        {
            var response = new BaseResponseGeneric<Comprobante_GlosaResponse>();

            try
            {
                string? glosa = string.Empty;

                // 🧩 CASO 1: ZIP con XML
                if (tipo?.Equals("02") == true)
                {
                    // 🔹 Caso 1️⃣: ZIP que contiene XML
                    try
                    {
                        // 1️⃣ Leer XML del ZIP
                        var xmlContent = _zipReader.ExtractXmlFromZip(zipFile);

                        // 2️⃣ Parsear XML a entidad de factura
                        var invoice = _xmlInvoiceParserService.ParseInvoice(xmlContent);

                        // 3️⃣ Obtener configuración global
                        var config = await _configuracionGlobalService.GetConfiguracionGlobalFirstOrDefaultAsync();
                        var max = config.MaxCaracteresGlosa > 0 ? config.MaxCaracteresGlosa : 100;

                        // 4️⃣ Construir glosa desde las descripciones del XML
                        glosa = string.Join("; ", invoice.InvoiceLines.Select(l =>
                        {
                            var desc = l.Description ?? string.Empty;
                            return desc.Length > max ? desc.Substring(0, max) + "..." : desc;
                        }));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error procesando XML para {Serie}-{Numero}", existingRecord.Serie, existingRecord.Numero);
                        glosa = null;
                    }
                }

                // 🧩 CASO 2: JSON directo (ConsultaCpeConsultaResponse)
                else if (tipo is null)
                {
                    // 🔹 Caso 2️⃣: ZIP que contiene JSON (ConsultaCpeConsultaResponse)
                    try
                    {
                        // 1️⃣ Leer JSON del ZIP
                        var jsonContent = System.Text.Encoding.UTF8.GetString(zipFile);

                        // 2️⃣ Deserializar al modelo ConsultaCpeConsultaResponse
                        var consultaResponse = JsonConvert.DeserializeObject<ConsultaCpeConsultaResponse>(jsonContent);

                        if (consultaResponse?.Comprobantes == null || !consultaResponse.Comprobantes.Any())
                        {
                            glosa = "(B) Sin información de comprobantes en el JSON.";
                        }
                        else
                        {
                            var comprobante = consultaResponse.Comprobantes.First(); // suele venir solo uno
                            var items = comprobante.InformacionItems ?? new List<InformacionItemCpe>();

                            // 3️⃣ Obtener configuración global
                            var config = await _configuracionGlobalService.GetConfiguracionGlobalFirstOrDefaultAsync();
                            var max = config.MaxCaracteresGlosa > 0 ? config.MaxCaracteresGlosa : 100;

                            // 4️⃣ Construir glosa desde las descripciones de los ítems
                            glosa = string.Join("; ", items.Select(i =>
                            {
                                var desc = i.DesItem ?? string.Empty;
                                return desc.Length > max ? desc.Substring(0, max) + "..." : desc;
                            }));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error procesando JSON para {Serie}-{Numero}", existingRecord.Serie, existingRecord.Numero);
                        glosa = null;
                    }
                }

                // 🧩 CASO 3: ZIP con PDF
                else if (tipo?.Equals("01") == true)
                {
                    try
                    {
                        // 1️⃣ Extraer PDF del ZIP
                        var pdfBytes = _zipReader.ExtractPdfFromZip(zipFile);

                        // 2️⃣ Extraer texto
                        var pdfText = _pdfReader.ExtractTextFromPdf(pdfBytes);

                        // 3️⃣ Obtener configuración
                        var config = await _configuracionGlobalService.GetConfiguracionGlobalFirstOrDefaultAsync();
                        var max = config.MaxCaracteresGlosa > 0 ? config.MaxCaracteresGlosa : 100;

                        // 4️⃣ Buscar la sección de descripción de ítems
                        // Muchos PDFs SUNAT incluyen la palabra "DescripciónCantidadUnidad"
                        // así que cortamos desde allí hasta "SON:" (total en letras)
                        string descripcionSection = string.Empty;

                        var startIdx = pdfText.IndexOf("Descripción", StringComparison.OrdinalIgnoreCase);
                        var endIdx = pdfText.IndexOf("SON:", StringComparison.OrdinalIgnoreCase);

                        if (startIdx >= 0 && endIdx > startIdx)
                            descripcionSection = pdfText.Substring(startIdx, endIdx - startIdx);
                        else
                            descripcionSection = pdfText; // fallback: texto completo

                        // 5️⃣ Limpiar texto y extraer posibles ítems
                        descripcionSection = descripcionSection
                            .Replace("DescripciónCantidadUnidad MedidaICBPERCódigoValor Unitario", " ")
                            .Replace("DescripciónCantidadUnidadMedidaICBPERCódigoValor Unitario", " ")
                            .Replace("Unidad", " ")
                            .Replace("UNIDAD", " ")
                            .Replace("S/", " ")
                            .Replace("SOL", " ")
                            .Replace("SOLES", " ")
                            .Replace(":", " ")
                            .Replace("\r", " ")
                            .Replace("\n", " ")
                            .Replace("  ", " ");

                        // 6️⃣ Detectar frases que parezcan descripciones de producto
                        // Por ejemplo: “ALQUILER DE MAQUNARIA ORUGA,EL COSTO DE HORA MAQUINA ES DE...”
                        var matches = Regex.Matches(descripcionSection, @"([A-ZÁÉÍÓÚÑ][A-Za-zÁÉÍÓÚÑáéíóúñ0-9 ,\.-]{10,})");

                        var descripciones = matches
                            .Select(m => m.Value.Trim())
                            .Where(x => x.Length > 5 && !x.StartsWith("Descripción", StringComparison.OrdinalIgnoreCase))
                            .Distinct()
                            .Take(5)
                            .ToList();

                        // 7️⃣ Construir glosa
                        glosa = string.Join("; ", descripciones.Select(d =>
                        {
                            var desc = d.Length > max ? d.Substring(0, max) + "..." : d;
                            return desc;
                        }));

                        glosa = $"(PDF) {glosa}";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error procesando PDF para {Serie}-{Numero}", existingRecord.Serie, existingRecord.Numero);
                        glosa = null;
                    }
                }

                // 🔹 Validar que haya resultado
                if (string.IsNullOrWhiteSpace(glosa))
                {
                    return ResponseFactory.Error<Comprobante_GlosaResponse>(
                        "No se pudo generar la glosa a partir del archivo proporcionado.",
                        "GLOSA_NOT_GENERATED",
                        422 // Unprocessable Entity: la solicitud fue válida, pero no se pudo procesar el contenido
                    );
                }

                // 🔹 Guardar en BD
                existingRecord.Glosa = glosa;
                existingRecord.TieneGlosa = true;

                await _comprobanteRepository.UpdateAsync(existingRecord);
                await _comprobanteRepository.UnitOfWork.SaveChangesAsync();

                // 🔹 Construir respuesta exitosa
                response = ResponseFactory.Success(new Comprobante_GlosaResponse
                {
                    Id = existingRecord.Id,
                    Serie = existingRecord.Serie,
                    Numero = existingRecord.Numero,
                    Glosa = glosa,
                    NombreArchivo = $"{existingRecord.NumeroDocIdentidad}-{existingRecord.TipoComprobante}-{existingRecord.Serie}-{existingRecord.Numero}"
                },
                "Glosa procesada correctamente.",
                200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error procesando glosa para {Serie}-{Numero}", existingRecord.Serie, existingRecord.Numero);
                
                response = ResponseFactory.Error<Comprobante_GlosaResponse>(
                    $"Error inesperado al procesar la glosa: {ex.Message}",
                    "GLOSA_PROCESS_EXCEPTION",
                    500
                );
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
