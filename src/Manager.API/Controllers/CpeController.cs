using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;

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
        public async Task<IActionResult> ConsultaCpeComprobante([FromBody] ConsultaCpeRequest request)
        {
            // 1. Obtener token válido
            var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.clienteId);

            var result = await _cpeService.ConsultaCpeComprobanteAsync(token.AccessToken, new ConsultaCpeComprobanteRequest
            {
                RucEmisor = request.RucEmisor,
                TipoComprobante = request.TipoComprobante,
                Serie = request.Serie,
                Numero = request.Numero,
                Tipo = request.Tipo
            });

            return Ok(result);
        }

        [HttpPost("consultacpe-unificado")]
        public async Task<IActionResult> ConsultaCpeUnificado([FromBody] ConsultaCpeRequest request)
        {
            var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();

            // 1. Obtener token válido
            var token = await _tokenBaseService.GetOrGenerateActiveTokenBaseAsync(cuentaBaseSol.Id);

            // 1️⃣ Primer intento: ControlCpeConsultaXml
            var respXml = await _cpeService.ControlCpeConsultaXmlAsync(token.AccessToken, new ConsultaCpeComprobanteRequest
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
            var respComprobante = await _cpeService.ConsultaCpeComprobanteAsync(token.AccessToken, new ConsultaCpeComprobanteRequest
            {
                RucEmisor = request.RucEmisor,
                TipoComprobante = request.TipoComprobante,
                Serie = request.Serie,
                Numero = request.Numero,
                Tipo = "02"
            });

            if (respComprobante.EsExito)
            {
                return Ok(new ConsultaCpeUnificadoResponse
                {
                    EsExito = true,
                    StatusCode = respComprobante.StatusCode,
                    Archivo = respComprobante.Archivo,
                    NombreArchivo = respComprobante.NombreArchivo
                });
            }

            // 3️⃣ Si ambos fallan, unificamos errores
            var errores = new List<ErrorConsultaCpeResponse>();

            if (respXml.Errores != null)
                errores.AddRange(respXml.Errores.Select(e => new ErrorConsultaCpeResponse
                {
                    status = e.status,
                    message = e.message
                }));

            if (respComprobante.Errores != null)
                errores.AddRange(respComprobante.Errores.Select(e => new ErrorConsultaCpeResponse
                {
                    status = e.status,
                    message = e.message
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
