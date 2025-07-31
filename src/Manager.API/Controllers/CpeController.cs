using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;
using Manager.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CpeController : ControllerBase
    {
        private readonly ICpeService _cpeService;

        public CpeController(ICpeService cpeService)
        {
            _cpeService = cpeService;
        }

        [HttpPost("status-cdr")]
        public async Task<IActionResult> StatusCdr([FromBody] ConsultarCpeRequest request)
        {
            var result = await _cpeService.StatusCdrAsync(request);
            return Ok(result);
        }

        [HttpPost("controlcpe-consultaxml")]
        public async Task<IActionResult> ControlCpeConsultaXml([FromBody] ConsultaCpeComprobanteRequest request)
        {
            // El token lo recuperas desde el contexto si lo necesitas
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _cpeService.ControlCpeConsultaXmlAsync(token, request);
            return Ok(result);
        }

        [HttpPost("consultacpe-comprobante")]
        public async Task<IActionResult> ConsultaCpeComprobante([FromBody] ConsultaCpeComprobanteRequest request)
        {
            // El token lo recuperas desde el contexto si lo necesitas
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var result = await _cpeService.ConsultaCpeComprobanteAsync(token, request);
            return Ok(result);
        }

        [HttpPost("consultacpe-unificado")]
        public async Task<IActionResult> ConsultaCpeUnificado([FromBody] ConsultaCpeRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            // 1️⃣ Primer intento: ControlCpeConsultaXml
            var respXml = await _cpeService.ControlCpeConsultaXmlAsync(token, new ConsultaCpeComprobanteRequest
            {
                RucEmisor = request.RucEmisor,
                TipoComprobante = request.TipoComprobante,
                Serie = request.Serie,
                Numero = request.Numero
            });

            if (respXml.EsExito)
            {
                return Ok(new ConsultaCpeUnificadoResponse
                {
                    EsExito = true,
                    StatusCode = respXml.StatusCode,
                    Archivo = respXml.Archivo,
                    NombreArchivo = respXml.NombreArchivo
                });
            }

            // 2️⃣ Segundo intento: ConsultaCpeComprobante
            var respComprobante = await _cpeService.ConsultaCpeComprobanteAsync(token, new ConsultaCpeComprobanteRequest
            {
                RucEmisor = request.RucEmisor,
                TipoComprobante = request.TipoComprobante,
                Serie = request.Serie,
                Numero = request.Numero,
                Tipo = "02"
            });

            if (respComprobante.EsExito)
            {
                // Convertir ValArchivo (Base64) a byte[]
                var archivoBytes = Convert.FromBase64String(respComprobante.ValArchivo ?? "");

                return Ok(new ConsultaCpeUnificadoResponse
                {
                    EsExito = true,
                    StatusCode = respComprobante.StatusCode,
                    Archivo = archivoBytes,
                    NombreArchivo = respComprobante.NomArchivo
                });
            }

            // 3️⃣ Si ambos fallan, unificamos errores
            var errores = new List<ErrorConsultaCpeResponse>();

            if (respXml.Errores != null)
                errores.AddRange(respXml.Errores.Select(e => new ErrorConsultaCpeResponse
                {
                    Status = e.status,
                    Message = e.message
                }));

            if (respComprobante.Errores != null)
                errores.AddRange(respComprobante.Errores.Select(e => new ErrorConsultaCpeResponse
                {
                    Status = e.status,
                    Message = e.message
                }));

            return Ok(new ConsultaCpeUnificadoResponse
            {
                EsExito = false,
                StatusCode = respComprobante.StatusCode != 0 ? respComprobante.StatusCode : respXml.StatusCode,
                Errores = errores
            });
        }
    }
}
