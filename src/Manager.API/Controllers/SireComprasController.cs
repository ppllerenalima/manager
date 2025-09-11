using Manager.Domain.Entities.Enum;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.PerTributario;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Requests.Ticket;
using Manager.Domain.Responses;
using Manager.Domain.Responses.ErroresResponses;
using Manager.Domain.Responses.PerTributarioResponses;
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
        private readonly IPerTributarioService _perTributarioService;

        public SireComprasController(ISireComprasService sireComprasService, IClienteService clienteSunatService, ITokenService tokenService, ITicketService ticketService, IPerTributarioService perTributarioService)
        {
            _sireComprasService = sireComprasService;
            _clienteSunatService = clienteSunatService;
            _tokenService = tokenService;
            _ticketService = ticketService;
            _perTributarioService = perTributarioService;
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

        [HttpPost("importar-comprobantes")]
        public async Task<IActionResult> ImportarComprobantesDesdeSunatAsync([FromBody] ArchivoReporteRequest request)
        {
            try
            {
                // 1. Obtener token válido
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.ClienteId);

                // 2. Obtener o generar ticket
                var ticket = await _ticketService.GetOrGenerateActiveTicketAsync(
                    token.AccessToken,
                    new GetTicketRequest
                    {
                        clienteId = request.ClienteId,
                        perTributario = $"{request.Anio}{request.Mes:D2}"
                    });

                // 3. Validar estado del ticket
                if (ticket.CodEstadoEnvio != "06")
                {
                    return StatusCode(202,
                        $"El ticket {ticket.NumTicket} aún no está aceptado. Estado: {ticket.CodEstadoEnvio}");
                }

                // 4. Descargar archivo
                var archivoResponse = await DescargarArchivoReporteSunat(token.AccessToken, request, ticket);

                // 5. Procesar archivo si fue exitoso
                if (archivoResponse?.EsExito == true)
                {
                    var perTributarioResponse = await GuardarPerTributarioAsync(request, archivoResponse.Archivo);
                    return Ok(perTributarioResponse);
                }

                // 6. Manejar error 2244 (ticket inválido/expirado)
                if (EsError2244(archivoResponse))
                {
                    var nuevoTicket = await _ticketService.GetOrGenerateActiveTicketAsync(
                        token.AccessToken,
                        new GetTicketRequest
                        {
                            clienteId = request.ClienteId,
                            perTributario = $"{request.Anio}{request.Mes:D2}"
                        });

                    if (nuevoTicket.CodEstadoEnvio != "06")
                    {
                        return StatusCode(202,
                            $"Nuevo ticket {nuevoTicket.NumTicket} aún no aceptado. Estado: {nuevoTicket.CodEstadoEnvio}");
                    }

                    var archivoResponseRetry = await DescargarArchivoReporteSunat(token.AccessToken, request, nuevoTicket);
                    if (archivoResponseRetry?.EsExito == true)
                    {
                        var perTributarioResponse = await GuardarPerTributarioAsync(request, archivoResponseRetry.Archivo);
                        return Ok(perTributarioResponse);
                    }
                }

                // 7. Error genérico si no se pudo descargar
                var mensajeError = archivoResponse?.Errores?.FirstOrDefault()?.message
                    ?? "El archivo no está disponible todavía.";
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

        private async Task<DescargarArchivoReporteResponse> DescargarArchivoReporteSunat(string accessToken, ArchivoReporteRequest request, TicketResponse ticket)
        {
            return await _sireComprasService.DescargarArchivoReporteAsync(
                accessToken,
                new DescargarArchivoReporteRequest
                {
                    PerTributario = $"{request.Anio}{request.Mes:D2}",
                    NomArchivoReporte = ticket.NomArchivoReporte,
                    CodTipoArchivoReporte = ticket.CodTipoAchivoReporte,
                    NumTicket = ticket.NumTicket,
                    CodProceso = ticket.CodProceso
                });
        }

        // 🔹 Método auxiliar para guardar en BD
        private async Task<PerTributarioResponse> GuardarPerTributarioAsync(ArchivoReporteRequest request, byte[] archivoZip)
        {
            return await _perTributarioService.AddPerTributarioAsync(new AddPerTributarioRequest
            {
                ClienteId = request.ClienteId,
                anio = request.Anio,
                mes = request.Mes,
                TipoComprobante = TipoComprobanteEnum.Compra,
                archivoZip = archivoZip
            });
        }

        // 🔹 Método auxiliar para verificar error 2244
        private static bool EsError2244(DescargarArchivoReporteResponse? archivoResponse)
        {
            var errorJson = archivoResponse?.Errores?.FirstOrDefault()?.message;
            if (string.IsNullOrEmpty(errorJson)) return false;

            var parsedError = System.Text.Json.JsonSerializer.Deserialize<ArchivoReporteErrorMessage>(errorJson);
            return parsedError?.errors?.Any(e => e.cod == 2244) == true;
        }
    }
}
