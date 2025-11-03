using Manager.Domain.Requests.Comprobante;
using Manager.Domain.Responses.ComprobanteResponses;
using Manager.Domain.Services;
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
        public async Task<IActionResult> Get([FromQuery] Guid perTributarioId, [FromQuery] bool? tieneGlosa, [FromQuery] string? search, [FromQuery] PaginationRequestModel? pagination)
        {
            pagination ??= new PaginationRequestModel();

            // Traemos los comprobantes (IQueryable si es posible)
            var result = (await _comprobanteService.GetComprobantesAsync(perTributarioId, search ?? ""))
                         .AsQueryable();

            // Aplico filtro opcional de glosa
            if (tieneGlosa.HasValue)
                result = result.Where(z => z.TieneGlosa == tieneGlosa.Value);

            // Total filtrado
            var totalComprobantes = result.Count();

            // Paginación + orden
            var itemsOnPage = result
                .OrderBy(c => c.FechaEmision)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize)
                .ToList();

            var model = new PaginatedResponseModel<ComprobanteResponse>(
                pagination.PageIndex,
                pagination.PageSize,
                totalComprobantes,
                itemsOnPage
            );

            return Ok(model);
        }


        [HttpGet("contadores")]
        public async Task<IActionResult> GetContadores([FromQuery] Guid perTributarioId)
        {
            var comprobantes = await _comprobanteService.GetComprobantesAsync(perTributarioId, string.Empty);

            var contadores = new ContadoresResponse
            {
                ConGlosa = comprobantes.Count(c => c.TieneGlosa),
                SinGlosa = comprobantes.Count(c => !c.TieneGlosa),
                Total = comprobantes.Count()
            };

            var model = new BaseResponseGeneric<ContadoresResponse>
            {
                Success = true,
                Data = contadores
            };

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
        public async Task<IActionResult> ImportarGlosa([FromBody] Comprobante_ImportarGlosaRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.PerTributarioId == Guid.Empty || request.ClienteId == Guid.Empty)
                return BadRequest(new BaseResponse
                {
                    Success = false,
                    Message = "Request inválido. Debes enviar ClienteId y PerTributarioId.",
                    StatusCode = StatusCodes.Status400BadRequest
                });

            try
            {
                // 1️⃣ Obtener token válido
                var token = await _tokenService.GetOrGenerateActiveTokenAsync(request.ClienteId);

                // 2️⃣ Ejecutar servicio principal
                var result = await _comprobanteService.ImportarGlosaAsync(
                    request.PerTributarioId,
                    token.AccessToken,
                    cancellationToken);

                // 3️⃣ Construir respuesta uniforme usando BaseResponseGeneric
                var response = new BaseResponseGeneric<Comprobante_ImportarGlosaResponse>
                {
                    Success = result.Success,
                    Message = result.Message,
                    StatusCode = result.StatusCode,
                    Data = new Comprobante_ImportarGlosaResponse
                    {
                        TotalProcesados = result.Data?.Count ?? 0,
                        Exitosos = result.Data?.Count(x => x.Exito) ?? 0,
                        Fallidos = result.Data?.Count(x => !x.Exito) ?? 0,
                        Detalle = result.Data ?? new List<Comprobante_GlosaResponse>()
                    }
                };

                // ✅ Devuelve con el código HTTP adecuado
                return StatusCode(response.StatusCode, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al importar glosas para ClienteId {ClienteId}, Periodo {Periodo}",
                    request.ClienteId, request.PerTributarioId);

                return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
                {
                    Success = false,
                    Message = "Ocurrió un error al procesar la importación de glosas.",
                    ErrorCode = "GLOSA_IMPORT_EXCEPTION",
                    StatusCode = StatusCodes.Status500InternalServerError
                });
            }
        }



        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateGlosa(Guid id, EditComprobanteRequest request)
        {
            request.Id = id;
            var result = await _comprobanteService.EditComprobanteAsync(request);
            return Ok(result);
        }
    }
}
