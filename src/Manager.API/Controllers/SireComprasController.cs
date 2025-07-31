using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SireComprasController : ControllerBase
    {
        private readonly ISireComprasService _sireComprasService;
        private readonly IClienteService _clienteSunatService;

        public SireComprasController(ISireComprasService sireComprasService, IClienteService clienteSunatService)
        {
            _sireComprasService = sireComprasService;
            _clienteSunatService = clienteSunatService;
        }

        [HttpGet("{Id:Guid}/token")]
        public async Task<IActionResult> GetToken(Guid Id)
        {
            var cliente = await _clienteSunatService.GetClienteAsync(new GetClienteRequest { Id = Id });

            var token = await _sireComprasService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cliente.ClientId,
                ClientSecret = cliente.ClientSecret,
                Username = $"{cliente.Ruc}{cliente.Username}",
                Password = cliente.Password
            });

            return Ok(token);
        }

        [HttpPost("aceptar-propuesta")]
        public async Task<IActionResult> AceptarPropuesta([FromBody] AceptarPropuestaRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.PeriodoTributario))
            {
                return BadRequest("AccessToken y PeriodoTributario son obligatorios.");
            }

            try
            {
                var resultado = await _sireComprasService.AceptarPropuestaAsync(request);

                if (resultado.FueAceptada)
                    return Ok(resultado);

                // No fue aceptada pero tampoco es un error de servidor
                return UnprocessableEntity(resultado);
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en logs si deseas
                return StatusCode(500, new { mensaje = "Error interno al procesar la solicitud.", detalle = ex.Message });
            }
        }

        [HttpPost("descargar-propuesta")]
        public async Task<IActionResult> DescargarPropuesta([FromBody] DescargarPropuestaRequest request)
        {
            var resultado = await _sireComprasService.DescargarPropuestaRCEAsync(request);

            return Ok(resultado); // para devolverlo como JSON
        }

        [HttpPost("consultar-estado-ticket")]
        public async Task<IActionResult> ConsultarEstadoTicket([FromBody] ConsultarEstadoTicketRequest request)
        {
            var resultado = await _sireComprasService.ConsultarEstadoTicketAsync(request);
            return Ok(resultado); // para devolverlo como JSON
        }

        [HttpPost("descargar-archivo")]
        public async Task<IActionResult> DescargarArchivoReporte([FromBody] DescargarArchivoReporteRequest request)
        {
            var result = await _sireComprasService.DescargarArchivoReporteAsync(request);

            if (result.EsExito)
            {
                return File(result.Archivo, "application/zip", result.NombreArchivo);
            }
            else
            {
                return StatusCode(result.StatusCode, result);
            }
        }
    }
}
