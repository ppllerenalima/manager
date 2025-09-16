using Manager.Domain.Requests.Comprobante;
using Manager.Domain.Responses.ComprobanteResponses;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprobanteController : ControllerBase
    {
        private readonly IComprobanteService _comprobanteService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<ComprobanteController> _logger;

        public ComprobanteController(IComprobanteService comprobanteService, ITokenService tokenService, ILogger<ComprobanteController> logger)
        {
            _comprobanteService = comprobanteService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid PerTributarioId, [FromQuery] string search, [FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _comprobanteService.GetComprobantesAsync(PerTributarioId, search ?? "");

            var totalComprobantes = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.FechaEmision)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<ComprobanteResponse>(pagination.PageIndex, pagination.PageSize, totalComprobantes, itemsOnPage);

            return Ok(model);
        }

        /// <summary>
        /// Importa y actualiza la glosa de los comprobantes electrónicos de un periodo tributario específico.
        /// 
        /// Este proceso consulta el servicio de SUNAT para obtener los archivos XML comprimidos (ZIP),
        /// los descomprime, extrae la información de las líneas de factura y actualiza la glosa en la base de datos.
        /// </summary>
        /// <param name="request">
        /// Objeto con los parámetros necesarios para la importación:
        /// <list type="bullet">
        /// <item><description><c>ClienteId</c>: Identificador único del cliente.</description></item>
        /// <item><description><c>PerTributarioId</c>: Identificador del periodo tributario.</description></item>
        /// </list>
        /// </param>
        /// <param name="cancellationToken">
        /// Token opcional para cancelar la operación de manera anticipada.
        /// </param>
        /// <returns>
        /// Un resultado HTTP con los siguientes posibles códigos de estado:
        /// <list type="bullet">
        /// <item><description><c>200 OK</c>: Devuelve la lista de comprobantes procesados con su glosa actualizada.</description></item>
        /// <item><description><c>400 Bad Request</c>: Si los parámetros de entrada son inválidos.</description></item>
        /// <item><description><c>500 Internal Server Error</c>: Si ocurrió un error inesperado durante la importación.</description></item>
        /// </list>
        /// </returns>
        [HttpPost("importar-glosa")]
        public async Task<IActionResult> ImportarGlosa(
            [FromBody] Comprobante_ImportarGlosaRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.PerTributarioId == Guid.Empty || request.ClienteId == Guid.Empty)
                return BadRequest("Request inválido. Debes enviar ClienteId y PerTributarioId.");

            try
            {
                // 1. Obtener token válido
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.ClienteId);

                // 2. Ejecutar servicio principal
                var result = await _comprobanteService.ImportarGlosaAsync(request.PerTributarioId, token.AccessToken, cancellationToken);

                // 3. Respuesta uniforme
                return Ok(new
                {
                    Success = true,
                    Count = result.Count,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al importar glosas para ClienteId {ClienteId}, Periodo {Periodo}",
                    request.ClienteId, request.PerTributarioId);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Success = false,
                    Message = "Ocurrió un error al procesar la importación de glosas.",
                    Details = ex.Message
                });
            }
        }
    }
}
