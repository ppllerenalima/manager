using Manager.Domain.Entities.Enum;
using Manager.Domain.Requests.Ticket;
using Manager.Domain.Responses;
using Manager.Domain.Responses.ErroresResponses;
using Manager.Domain.Responses.PerTributarioResponses;
using Manager.Domain.Responses.TicketResponses;
using Manager.Domain.Services.Interfaces;

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
        private readonly ILogger<SireComprasController> _logger;

        public SireComprasController(ISireComprasService sireComprasService, IClienteService clienteSunatService, ITokenService tokenService, ITicketService ticketService, IPerTributarioService perTributarioService, ILogger<SireComprasController> logger)
        {
            _sireComprasService = sireComprasService;
            _clienteSunatService = clienteSunatService;
            _tokenService = tokenService;
            _ticketService = ticketService;
            _perTributarioService = perTributarioService;
            _logger = logger;
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


        /// <summary>
        /// Importa los comprobantes desde SUNAT para un periodo tributario determinado.
        /// </summary>
        /// <param name="request">Contiene el ClienteId, Año y Mes del periodo tributario.</param>
        /// <returns>
        /// Retorna un <see cref="BaseResponseGeneric{PerTributarioResponse}"/> con el resultado:
        /// - <c>Success = true</c> si los comprobantes se importaron y registraron correctamente.
        /// - <c>Success = false</c> si hubo errores durante la comunicación con SUNAT o la base de datos.
        /// </returns>
        [HttpPost("importar-comprobantes")]
        public async Task<IActionResult> ImportarComprobantesDesdeSunatAsync([FromBody] GetPerTributarioByPeriodoRequest request)
        {
            try
            {
                // 🧩 Paso 1: Obtener token válido
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.ClienteId);

                // 🧩 Paso 2: Obtener o generar ticket válido
                var ticket = await ObtenerTicketValidoAsync(token.AccessToken, request);
                if (ticket == null)
                    return StatusCode(202, "No se pudo obtener un ticket válido para este periodo.");

                // 🧩 Paso 3: Descargar archivo de reporte SUNAT
                var archivoResponse = await DescargarArchivoValidoAsync(token.AccessToken, ticket, request);
                if (!archivoResponse.Success || archivoResponse.Data?.Archivo == null)
                    return StatusCode(502, archivoResponse.Message ?? "El archivo no está disponible todavía.");

                // 🧩 Paso 4: Registrar periodo y comprobantes
                var perTributarioResponse = await _perTributarioService.AddPerTributarioAsync(new AddPerTributarioRequest
                {
                    ClienteId = request.ClienteId,
                    anio = request.Anio,
                    mes = request.Mes,
                    TipoComprobante = TipoComprobanteEnum.Compra,
                    archivoZip = archivoResponse.Data.Archivo
                });

                return StatusCode(perTributarioResponse.StatusCode, perTributarioResponse);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Cliente no encontrado al importar comprobantes.");
                return NotFound(new BaseResponse { Success = false, Message = ex.Message, StatusCode = 404 });
            }
            catch (ApplicationException ex)
            {
                _logger.LogWarning(ex, "Error de aplicación al importar comprobantes.");
                return StatusCode(502, new BaseResponse { Success = false, Message = ex.Message, StatusCode = 502 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al importar comprobantes.");
                return StatusCode(500, new BaseResponse { Success = false, Message = "Error interno del servidor.", StatusCode = 500 });
            }
        }

        private async Task<TicketResponse?> ObtenerTicketValidoAsync(string token, GetPerTributarioByPeriodoRequest request)
        {
            var ticket = await _ticketService.GetOrGenerateActiveTicketAsync(token, new GetTicketRequest
            {
                clienteId = request.ClienteId,
                perTributario = $"{request.Anio}{request.Mes:D2}"
            });

            // ✅ Si el ticket aún no está aceptado, avisamos
            if (ticket.CodEstadoEnvio != "06")
            {
                _logger.LogInformation("Ticket {Ticket} aún no aceptado (Estado {Estado})", ticket.NumTicket, ticket.CodEstadoEnvio);
                return null;
            }

            return ticket;
        }

        private async Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoValidoAsync(
            string token,
            TicketResponse ticket,
            GetPerTributarioByPeriodoRequest request)
        {
            var archivoResponse = await _sireComprasService.DescargarArchivoReporteAsync(token, new DescargarArchivoReporteRequest
            {
                PerTributario = $"{request.Anio}{request.Mes:D2}",
                NomArchivoReporte = ticket.NomArchivoReporte,
                CodTipoArchivoReporte = ticket.CodTipoAchivoReporte,
                NumTicket = ticket.NumTicket,
                CodProceso = ticket.CodProceso
            });

            // ⚙️ Si hay error 2244 (ticket expirado), intentar uno nuevo automáticamente
            if (!archivoResponse.Success && EsError2244(archivoResponse.Data))
            {
                _logger.LogWarning("Ticket {Ticket} expirado. Solicitando nuevo ticket...", ticket.NumTicket);

                var nuevoTicket = await ObtenerTicketValidoAsync(token, request);
                if (nuevoTicket == null) return archivoResponse;

                return await _sireComprasService.DescargarArchivoReporteAsync(token, new DescargarArchivoReporteRequest
                {
                    PerTributario = $"{request.Anio}{request.Mes:D2}",
                    NomArchivoReporte = nuevoTicket.NomArchivoReporte,
                    CodTipoArchivoReporte = nuevoTicket.CodTipoAchivoReporte,
                    NumTicket = nuevoTicket.NumTicket,
                    CodProceso = nuevoTicket.CodProceso
                });
            }

            return archivoResponse;
        }

        //[HttpPost("importar-comprobantes")]
        //public async Task<IActionResult> ImportarComprobantesDesdeSunatAsync([FromBody] GetPerTributarioByPeriodoRequest request)
        //{
        //    try
        //    {
        //        // 1. Obtener token válido
        //        var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.ClienteId);

        //        // 2. Obtener o generar ticket
        //        var ticket = await _ticketService.GetOrGenerateActiveTicketAsync(
        //            token.AccessToken,
        //            new GetTicketRequest
        //            {
        //                clienteId = request.ClienteId,
        //                perTributario = $"{request.Anio}{request.Mes:D2}"
        //            });

        //        // 3. Validar estado del ticket
        //        if (ticket.CodEstadoEnvio != "06")
        //        {
        //            return StatusCode(202,
        //                $"El ticket {ticket.NumTicket} aún no está aceptado. Estado: {ticket.CodEstadoEnvio}");
        //        }

        //        // 4. Descargar archivo
        //        var archivoResponse = await _sireComprasService.DescargarArchivoReporteAsync(
        //                token.AccessToken,
        //                new DescargarArchivoReporteRequest
        //                {
        //                    PerTributario = $"{request.Anio}{request.Mes:D2}",
        //                    NomArchivoReporte = ticket.NomArchivoReporte,
        //                    CodTipoArchivoReporte = ticket.CodTipoAchivoReporte,
        //                    NumTicket = ticket.NumTicket,
        //                    CodProceso = ticket.CodProceso
        //                });

        //        // 5. Procesar archivo si fue exitoso
        //        if (archivoResponse.Success)
        //        {
        //            var perTributarioResponse = await _perTributarioService.AddPerTributarioAsync(new AddPerTributarioRequest
        //            {
        //                ClienteId = request.ClienteId,
        //                anio = request.Anio,
        //                mes = request.Mes,
        //                TipoComprobante = TipoComprobanteEnum.Compra,
        //                archivoZip = archivoResponse.Data.Archivo
        //            });

        //            return StatusCode(perTributarioResponse.StatusCode, perTributarioResponse);
        //        }

        //        // 6. Manejar error 2244 (ticket inválido/expirado)
        //        if (EsError2244(archivoResponse.Data))
        //        {
        //            var nuevoTicket = await _ticketService.GetOrGenerateActiveTicketAsync(
        //                token.AccessToken,
        //                new GetTicketRequest
        //                {
        //                    clienteId = request.ClienteId,
        //                    perTributario = $"{request.Anio}{request.Mes:D2}"
        //                });

        //            if (nuevoTicket.CodEstadoEnvio != "06")
        //            {
        //                return StatusCode(202,
        //                    $"Nuevo ticket {nuevoTicket.NumTicket} aún no aceptado. Estado: {nuevoTicket.CodEstadoEnvio}");
        //            }

        //            var archivoResponseRetry = await _sireComprasService.DescargarArchivoReporteAsync(
        //               token.AccessToken,
        //               new DescargarArchivoReporteRequest
        //               {
        //                   PerTributario = $"{request.Anio}{request.Mes:D2}",
        //                   NomArchivoReporte = nuevoTicket.NomArchivoReporte,
        //                   CodTipoArchivoReporte = nuevoTicket.CodTipoAchivoReporte,
        //                   NumTicket = nuevoTicket.NumTicket,
        //                   CodProceso = nuevoTicket.CodProceso
        //               });

        //            if (archivoResponseRetry.Success)
        //            {
        //                var perTributarioResponse = await _perTributarioService.AddPerTributarioAsync(new AddPerTributarioRequest
        //                {
        //                    ClienteId = request.ClienteId,
        //                    anio = request.Anio,
        //                    mes = request.Mes,
        //                    TipoComprobante = TipoComprobanteEnum.Compra,
        //                    archivoZip = archivoResponseRetry.Data.Archivo
        //                });

        //                return StatusCode(perTributarioResponse.StatusCode, perTributarioResponse);
        //            }
        //        }

        //        // 7. Error genérico si no se pudo descargar
        //        var mensajeError = archivoResponse?.Message
        //            ?? "El archivo no está disponible todavía.";
        //        return StatusCode(502, mensajeError);
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);
        //    }
        //    catch (ApplicationException ex)
        //    {
        //        return StatusCode(502, ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error interno: {ex.Message}");
        //    }
        //}

        //private async Task<DescargarArchivoReporteResponse> DescargarArchivoReporteSunat(string accessToken, GetPerTributarioByPeriodoRequest request, TicketResponse ticket)
        //{
        //    return await _sireComprasService.DescargarArchivoReporteAsync(
        //        accessToken,
        //        new DescargarArchivoReporteRequest
        //        {
        //            PerTributario = $"{request.Anio}{request.Mes:D2}",
        //            NomArchivoReporte = ticket.NomArchivoReporte,
        //            CodTipoArchivoReporte = ticket.CodTipoAchivoReporte,
        //            NumTicket = ticket.NumTicket,
        //            CodProceso = ticket.CodProceso
        //        });
        //}

        // 🔹 Método auxiliar para guardar en BD
        //private async Task<PerTributarioResponse> GuardarPerTributarioAsync(GetPerTributarioByPeriodoRequest request, byte[] archivoZip)
        //{
        //    return await _perTributarioService.AddPerTributarioAsync(new AddPerTributarioRequest
        //    {
        //        ClienteId = request.ClienteId,
        //        anio = request.Anio,
        //        mes = request.Mes,
        //        TipoComprobante = TipoComprobanteEnum.Compra,
        //        archivoZip = archivoZip
        //    });
        //}

        // 🔹 Método auxiliar para verificar error 2244
        private static bool EsError2244(DescargarArchivoReporteResponse? archivoResponse)
        {
            var errorJson = archivoResponse?.ErrorContent;
            if (string.IsNullOrEmpty(errorJson)) return false;

            var parsedError = System.Text.Json.JsonSerializer.Deserialize<ArchivoReporteErrorMessage>(errorJson);
            return parsedError?.errors?.Any(e => e.cod == 2244) == true;
        }
    }
}
