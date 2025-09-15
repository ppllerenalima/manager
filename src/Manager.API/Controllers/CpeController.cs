using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;
using Manager.Domain.Services.Interfaces;
using System.IO.Compression;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CpeController : ControllerBase
    {
        private readonly ICpeService _cpeService;
        private readonly ITokenService _tokenService;
        private readonly ITokenBaseService _tokenBaseService;
        private readonly ICuentaBaseSolService _cuentaBaseSolService;


        public CpeController(ICpeService cpeService, ITokenService tokenService, ITokenBaseService tokenBaseService, ICuentaBaseSolService cuentaBaseSolService)
        {
            _cpeService = cpeService;
            _tokenService = tokenService;
            _tokenBaseService = tokenBaseService;
            _cuentaBaseSolService = cuentaBaseSolService;
        }

        /// <summary>
        /// Descarga el archivo ZIP asociado a un comprobante electrónico,
        /// utilizando los servicios de la SUNAT (ControlCPE y ConsultaCPE).
        /// </summary>
        /// <param name="request">
        /// Objeto con los parámetros necesarios:
        /// - <c>RucEmisor</c>: RUC del emisor del comprobante.
        /// - <c>TipoComprobante</c>: Tipo de CPE (01=Factura, 03=Boleta, etc.).
        /// - <c>Serie</c>: Serie del comprobante.
        /// - <c>Numero</c>: Número del comprobante.
        /// - <c>Tipo</c>: Tipo de archivo a descargar (01=PDF, 02=XML, etc.).
        /// </param>
        /// <returns>
        /// Un resultado HTTP con los siguientes posibles estados:
        /// - <c>200 OK</c>: Descarga exitosa. Devuelve el archivo en base64 junto con metadatos.
        /// - <c>400 Bad Request</c>: Error en los parámetros de entrada.
        /// - <c>500 Internal Server Error</c>: Error inesperado o fallo en los servicios de SUNAT.
        /// </returns>
        [HttpPost("DescargarZip")]
        public async Task<IActionResult> DescargarZip([FromBody] DescargarZipRequest request)
        {
            if (request == null)
            {
                return BadRequest(new DescargarZipResponse
                {
                    EsExito = false,
                    StatusCode = 400,
                    Errores = new List<ErrorDescargarZipResponse>
            {
                new ErrorDescargarZipResponse
                {
                    status = "REQ_NULL",
                    message = "La solicitud no puede ser nula."
                }
            }
                });
            }

            try
            {
                // 1️⃣ Obtener cuenta y token
                var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();
                if (cuentaBaseSol == null)
                {
                    return BadRequest(new DescargarZipResponse
                    {
                        EsExito = false,
                        StatusCode = 400,
                        Errores = new List<ErrorDescargarZipResponse>
                {
                    new ErrorDescargarZipResponse
                    {
                        status = "NO_CUENTA",
                        message = "No se encontró configuración de cuenta BaseSol."
                    }
                }
                    });
                }

                var token = await _tokenBaseService.GetOrGenerateActiveTokenBaseAsync(cuentaBaseSol.Id);

                // 2️⃣ Descargar usando la lógica de fallback (ControlCpe → ConsultaCpe)
                var response = await _cpeService.DescargarZipAsync(token.AccessToken, request);

                // 3️⃣ Responder según éxito o error
                if (response.EsExito)
                {

                    return ProcesarPdfDesdeZip(response);
                }

                return StatusCode(response.StatusCode > 0 ? response.StatusCode : 500, response);
            }
            catch (Exception ex)
            {
                // 4️⃣ Manejo global de excepciones
                return StatusCode(500, new DescargarZipResponse
                {
                    EsExito = false,
                    StatusCode = 500,
                    Errores = new List<ErrorDescargarZipResponse>
            {
                new ErrorDescargarZipResponse
                {
                    status = "EX",
                    message = ex.Message
                }
            }
                });
            }
        }

        private IActionResult ProcesarPdfDesdeZip(DescargarZipResponse result)
        {
            try
            {
                using var zipStream = new MemoryStream(result.Archivo);
                using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

                var entry = archive.Entries.FirstOrDefault(e =>
                    e.FullName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase));

                if (entry == null)
                    return ErrorResponse(404, "NO_PDF", "No se encontró un PDF dentro del ZIP.");

                using var entryStream = entry.Open();
                using var ms = new MemoryStream();
                entryStream.CopyTo(ms);
                ms.Position = 0; // 🔑 necesario

                return File(ms.ToArray(), "application/pdf", entry.Name);
            }
            catch (Exception ex)
            {
                return ErrorResponse(500, "EX", ex.Message);
            }
        }

        private IActionResult ErrorResponse(int statusCode, string status, string message)
        {
            return StatusCode(statusCode, new DescargarZipResponse
            {
                EsExito = false,
                StatusCode = statusCode,
                Errores = new List<ErrorDescargarZipResponse>
                {
                    new ErrorDescargarZipResponse { status = status, message = message }
                }
            });
        }


        //[HttpPost("status-cdr")]
        //public async Task<IActionResult> StatusCdr([FromBody] ConsultarCpeRequest request)
        //{
        //    var result = await _cpeService.StatusCdrAsync(request);
        //    return Ok(result);
        //}

        //[HttpPost("controlcpe-consultaxml")]
        //public async Task<IActionResult> ControlCpeConsultaXml([FromBody] DescargarZipRequest request)
        //{
        //    // El token lo recuperas desde el contexto si lo necesitas
        //    var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        //    var result = await _cpeService.ControlCpeConsultaXmlAsync(token, request);
        //    return Ok(result);
        //}

        //[HttpPost("consultacpe-comprobante")]
        //public async Task<IActionResult> ConsultaCpeComprobante([FromBody] ConsultaCpeRequest request)
        //{
        //    // 1. Obtener token válido
        //    var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.clienteId);

        //    var result = await _cpeService.ConsultaCpeComprobanteAsync(token.AccessToken, new DescargarZipRequest
        //    {
        //        RucEmisor = request.RucEmisor,
        //        TipoComprobante = request.TipoComprobante,
        //        Serie = request.Serie,
        //        Numero = request.Numero,
        //        Tipo = request.Tipo
        //    });

        //    return Ok(result);
        //}

        //[HttpPost("consultacpe-unificado")]
        //public async Task<IActionResult> ConsultaCpeUnificado([FromBody] ConsultaCpeRequest request)
        //{
        //    var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();

        //    // 1. Obtener token válido
        //    var token = await _tokenBaseService.GetOrGenerateActiveTokenBaseAsync(cuentaBaseSol.Id);

        //    // 1️⃣ Primer intento: ControlCpeConsultaXml
        //    var respXml = await _cpeService.ControlCpeConsultaXmlAsync(token.AccessToken, new DescargarZipRequest
        //    {
        //        RucEmisor = request.RucEmisor,
        //        TipoComprobante = request.TipoComprobante,
        //        Serie = request.Serie,
        //        Numero = request.Numero
        //    });

        //    if (respXml.EsExito)
        //    {
        //        return Ok(new DescargarZipResponse
        //        {
        //            EsExito = true,
        //            StatusCode = respXml.StatusCode,
        //            Archivo = respXml.Archivo,
        //            NombreArchivo = respXml.NombreArchivo
        //        });
        //    }

        //    // 2️⃣ Segundo intento: ConsultaCpeComprobante
        //    var respComprobante = await _cpeService.ConsultaCpeComprobanteAsync(token.AccessToken, new DescargarZipRequest
        //    {
        //        RucEmisor = request.RucEmisor,
        //        TipoComprobante = request.TipoComprobante,
        //        Serie = request.Serie,
        //        Numero = request.Numero,
        //        Tipo = "02"
        //    });

        //    if (respComprobante.EsExito)
        //    {
        //        return Ok(new DescargarZipResponse
        //        {
        //            EsExito = true,
        //            StatusCode = respComprobante.StatusCode,
        //            Archivo = respComprobante.Archivo,
        //            NombreArchivo = respComprobante.NombreArchivo
        //        });
        //    }

        //    // 3️⃣ Si ambos fallan, unificamos errores
        //    var errores = new List<ErrorConsultaCpeResponse>();

        //    if (respXml.Errores != null)
        //        errores.AddRange(respXml.Errores.Select(e => new ErrorConsultaCpeResponse
        //        {
        //            status = e.status,
        //            message = e.message
        //        }));

        //    if (respComprobante.Errores != null)
        //        errores.AddRange(respComprobante.Errores.Select(e => new ErrorConsultaCpeResponse
        //        {
        //            status = e.status,
        //            message = e.message
        //        }));

        //    return Ok(new DescargarZipResponse
        //    {
        //        EsExito = false,
        //        StatusCode = respComprobante.StatusCode != 0 ? respComprobante.StatusCode : respXml.StatusCode,
        //        Errores = errores
        //    });
        //}

        //[HttpPost("consultacpe-lote")]
        //public async Task<IActionResult> ConsultaCpeLote([FromBody] List<ConsultaCpeRequest> requests)
        //{
        //    var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();
        //    var token = await _tokenBaseService.GetOrGenerateActiveTokenBaseAsync(cuentaBaseSol.Id);

        //    var comprobantes = requests.Select(r => new DescargarZipRequest
        //    {
        //        RucEmisor = r.RucEmisor,
        //        TipoComprobante = r.TipoComprobante,
        //        Serie = r.Serie,
        //        Numero = r.Numero,
        //        Tipo = "02"
        //    }).ToList();

        //    var results = await _cpeService.ConsultarLoteCpeAsync(token.AccessToken, comprobantes);

        //    return Ok(results);
        //}

    }
}
