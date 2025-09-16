using Manager.Domain.Services.Interfaces;
using System.Collections.Concurrent;

namespace Manager.Domain.Services
{
    public class ComprobanteService : IComprobanteService
    {
        private readonly IMapper _comprobanteMapper;
        private readonly IComprobanteRepository _comprobanteRepository;
        private readonly IZipReader _zipReader;
        private readonly IXmlInvoiceParserService _xmlInvoiceParserService;

        private readonly ICpeService _cpeService;

        private readonly ILogger<ComprobanteService> _logger;

        public ComprobanteService(IComprobanteRepository comprobanteRepository, IZipReader zipReader, IXmlInvoiceParserService xmlInvoiceParserService, ICpeService cpeService, IMapper comprobanteMapper, ILogger<ComprobanteService> logger)
        {
            _comprobanteRepository = comprobanteRepository;
            _zipReader = zipReader;
            _xmlInvoiceParserService = xmlInvoiceParserService;
            _cpeService = cpeService;
            _comprobanteMapper = comprobanteMapper;
            _logger = logger;
        }

        public async Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync(Guid perTributarioId, string search)
        {
            var result = await _comprobanteRepository.GetAsync(predicate: z => z.PerTributarioId == perTributarioId && (z.NombreProveedor.Contains(search) || z.NumeroDocIdentidad.Contains(search) || z.Serie.Contains(search) || z.Numero.Contains(search)));

            return result
                .Select(x => _comprobanteMapper.Map<ComprobanteResponse>(x));
        }

        public async Task<ICollection<Comrpobante_GlosaResponse>> ImportarGlosaAsync(Guid perTributarioId, string token, CancellationToken cancellationToken)
        {
            int maxConcurrent = 5;

            var comprobantes = await _comprobanteRepository.GetAsync(predicate: z => z.PerTributarioId == perTributarioId);

            var results = new ConcurrentBag<Comrpobante_GlosaResponse>();

            await Parallel.ForEachAsync(comprobantes,
                new ParallelOptions { MaxDegreeOfParallelism = maxConcurrent, CancellationToken = cancellationToken },
                async (cpe, ct) =>
                {
                    // 🔽 Descarga el ZIP con resiliencia
                    var zipBytes = await EjecutarConResilienciaAsync(
                        () => _cpeService.DescargarZipAsync(token, new DescargarZipRequest
                        {
                            RucEmisor = cpe.NumeroDocIdentidad,
                            TipoComprobante = cpe.TipoComprobante,
                            Numero = Convert.ToInt32(cpe.Numero),
                            Serie = cpe.Serie,
                            Tipo = "02"
                        }));

                    // 🔽 Orquestador hace todo: leer → parsear → guardar
                    var resultado = await ProcesarAsync(zipBytes.Archivo, cpe, ct);

                    results.Add(resultado);
                });


            return results.ToList();
        }

        private async Task<Comrpobante_GlosaResponse> ProcesarAsync(byte[] zipFile, Comprobante existinfRecord, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Leer ZIP
                var xmlContent = _zipReader.ExtractXmlFromZip(zipFile);

                // 2. Parsear XML
                var invoice = _xmlInvoiceParserService.ParseInvoice(xmlContent);

                // 3. Obtener Glosa
                var glosa = string.Join("; ", invoice.InvoiceLines.Select(l => l.Description.Length > 15 ? l.Description.Substring(0, 15) : l.Description));
                existinfRecord.Glosa = glosa;
                existinfRecord.TieneGlosa = true;

                // 4. Guardar en BD
                await _comprobanteRepository.UpdateAsync(existinfRecord);
                await _comprobanteRepository.UnitOfWork.SaveChangesAsync();

                return new Comrpobante_GlosaResponse
                {
                    EsExito = true,
                    StatusCode = 200,
                    NombreArchivo = $"{existinfRecord.NumeroDocIdentidad}-{existinfRecord.TipoComprobante}-{existinfRecord.Serie}-{existinfRecord.Numero}"
                };
            }
            catch (Exception ex)
            {
                return new Comrpobante_GlosaResponse
                {
                    EsExito = false,
                    StatusCode = 500,
                    NombreArchivo = $"{existinfRecord.NumeroDocIdentidad}-{existinfRecord.TipoComprobante}-{existinfRecord.Serie}-{existinfRecord.Numero}",
                    Errores = new List<ErrorComrpobante_Response>
                    {
                        new ErrorComrpobante_Response
                        {
                            Status = "EX",
                            Message = ex.Message
                        }
                    }
                };
            }
        }

        private async Task<T> EjecutarConResilienciaAsync<T>(
            Func<Task<T>> accion,
            int maxIntentos = 3,
            int delayMs = 1000)
        {
            int intento = 0;
            Exception ultimaEx = null;

            while (intento < maxIntentos)
            {
                try
                {
                    return await accion();
                }
                catch (Exception ex)
                {
                    ultimaEx = ex;
                    intento++;
                    if (intento < maxIntentos)
                    {
                        await Task.Delay(delayMs * intento); // backoff exponencial
                    }
                }
            }

            throw ultimaEx ?? new Exception("Error desconocido en resiliencia");
        }
    }
}
