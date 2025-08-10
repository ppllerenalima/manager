using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Requests.Ticket;
using Manager.Domain.Responses;
using Manager.Domain.Responses.ErroresResponses;
using Manager.Domain.Responses.TicketResponses;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SireComprasController : ControllerBase
    {
        private readonly ISireComprasService _sireComprasService;
        private readonly IClienteService _clienteSunatService;
        private readonly ITokenService _tokenService;
        private readonly ITicketService _ticketService;


        public SireComprasController(ISireComprasService sireComprasService, IClienteService clienteSunatService, ITokenService tokenService, ITicketService ticketService)
        {
            _sireComprasService = sireComprasService;
            _clienteSunatService = clienteSunatService;
            _tokenService = tokenService;
            _ticketService = ticketService;
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
        public async Task<IActionResult> DescargarArchivoReporte([FromBody] ArchivoReporteRequest request)
        {
            try
            {
                // 1. Obtener token válido
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.clienteId);

                // 2. Obtener o generar ticket
                var ticket = await _ticketService.GetOrGenerateActiveTicketAsync(
                    token.AccessToken,
                    new GetTicketRequest
                    {
                        clienteId = request.clienteId,
                        perTributario = request.perTributario
                    });

                // 3. Validar estado del ticket
                if (ticket.CodEstadoEnvio != "06")
                    return StatusCode(202, $"El ticket {ticket.NumTicket} aún no está aceptado. Estado: {ticket.CodEstadoEnvio}");

                // 4. Intentar descargar archivo
                var archivoResponse = await DescargarArchivoReporteSunat(token.AccessToken, request, ticket);

                // 5. Evaluar respuesta
                if (archivoResponse?.EsExito == true)
                {
                    return File(archivoResponse.Archivo, "application/zip", archivoResponse.NombreArchivo ?? "archivo.zip");
                }

                // 6. Verificar si el error es 2244
                var errorJson = archivoResponse?.Errores?.FirstOrDefault()?.message;
                if (!string.IsNullOrEmpty(errorJson))
                {
                    var parsedError = System.Text.Json.JsonSerializer.Deserialize<ArchivoReporteErrorMessage>(errorJson);
                    if (parsedError?.errors?.Any(e => e.cod == 2244) == true)
                    {
                        // 🔹 Regenerar ticket
                        var nuevoTicket = await _ticketService.GetOrGenerateActiveTicketAsync(
                            token.AccessToken,
                            new GetTicketRequest
                            {
                                clienteId = request.clienteId,
                                perTributario = request.perTributario
                            }, true);

                        if (nuevoTicket.CodEstadoEnvio != "06")
                            return StatusCode(202, $"Nuevo ticket {nuevoTicket.NumTicket} aún no aceptado. Estado: {nuevoTicket.CodEstadoEnvio}");

                        // 🔹 Reintentar descarga con el nuevo ticket
                        var archivoResponse2 = await DescargarArchivoReporteSunat(token.AccessToken, request, nuevoTicket);
                        if (archivoResponse2?.EsExito == true)
                        {
                            return File(archivoResponse2.Archivo, "application/zip", archivoResponse2.NombreArchivo ?? "archivo.zip");
                        }
                    }
                }

                // 7. Si llega aquí, no se pudo descargar
                var mensajeError = archivoResponse?.Errores?.FirstOrDefault()?.message ?? "El archivo no está disponible todavía.";

                return StatusCode(502, mensajeError);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(502, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        private async Task<DescargarArchivoReporteResponse> DescargarArchivoReporteSunat(
            string accessToken,
            ArchivoReporteRequest request,
            TicketResponse ticket)
        {
            return await _sireComprasService.DescargarArchivoReporteAsync(
                accessToken,
                new DescargarArchivoReporteRequest
                {
                    PerTributario = request.perTributario,
                    NomArchivoReporte = ticket.NomArchivoReporte,
                    CodTipoArchivoReporte = ticket.CodTipoAchivoReporte,
                    NumTicket = ticket.NumTicket,
                    CodProceso = ticket.CodProceso
                });
        }
    }
}
