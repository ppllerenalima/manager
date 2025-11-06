namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SireComprasController : ControllerBase
    {
        private readonly ISireComprasService _sireComprasService;

        private readonly ITokenService _tokenService;
        private readonly ITicketService _ticketService;
        private readonly IPerTributarioService _perTributarioService;
        
        private readonly ILogger<SireComprasController> _logger;

        public SireComprasController(ISireComprasService sireComprasService, ITokenService tokenService, ITicketService ticketService, IPerTributarioService perTributarioService, ILogger<SireComprasController> logger)
        {
            _sireComprasService = sireComprasService;
            
            _tokenService = tokenService;
            _ticketService = ticketService;
            _perTributarioService = perTributarioService;

            _logger = logger;
        }

        //[HttpGet("{Id:Guid}/token")]
        //public async Task<IActionResult> GetToken(Guid Id)
        //{
        //    var cliente = await _clienteSunatService.GetClienteAsync(new GetClienteRequest { Id = Id });

        //    var token = await _sireComprasService.AccessTokenAsync(new SunatAuthRequest
        //    {
        //        ClientId = cliente.ClientId,
        //        ClientSecret = cliente.ClientSecret,
        //        Username = $"{cliente.Ruc}{cliente.Username}",
        //        Password = cliente.Password
        //    });

        //    return Ok(token);
        //}

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
                if (!token.Success || string.IsNullOrEmpty(token.Data?.AccessToken)) 
                    return StatusCode(token.StatusCode, token.Message);

                // 🧩 Paso 2: Obtener o generar ticket válido
                var ticket = await _ticketService.GetOrGenerateActiveTicketAsync(token.Data.AccessToken, new GetTicketRequest
                {
                    clienteId = request.ClienteId,
                    perTributario = $"{request.Anio}{request.Mes:D2}"
                });
                if(!ticket.Success)
                    return StatusCode(ticket.StatusCode, ticket.Message);

                if (!ticket.Data.CodEstadoEnvio.Equals("06"))
                    return StatusCode(409, $"El ticket obtenido está en estado {ticket.Data.DesEstadoEnvio.ToUpper()}. Código de estado de envío: {ticket.Data.CodEstadoEnvio}");

                // 🧩 Paso 3: Descargar archivo de reporte SUNAT
                var archivoResponse = await _sireComprasService.DescargarArchivoReporteAsync(token.Data.AccessToken, request.ClienteId, new DescargarArchivoReporteRequest
                {
                    PerTributario = $"{request.Anio}{request.Mes:D2}",
                    NomArchivoReporte = ticket.Data!.NomArchivoReporte,
                    CodTipoArchivoReporte = ticket.Data!.CodTipoAchivoReporte,
                    NumTicket = ticket.Data!.NumTicket,
                    CodProceso = ticket.Data!.CodProceso
                });

                if (!archivoResponse.Success || archivoResponse.Data?.Archivo == null)
                    return StatusCode(archivoResponse.StatusCode, $"{archivoResponse.Message} - {archivoResponse.Details}");

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
    }
}
