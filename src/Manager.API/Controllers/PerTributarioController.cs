using Manager.Domain.Responses.PerTributarioResponses;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerTributarioController : ControllerBase
    {
        private readonly IPerTributarioService _perTributarioService;

        public PerTributarioController(IPerTributarioService perTributarioService)
        {
            _perTributarioService = perTributarioService;
        }

        [HttpGet("{id:guid}")]
        //[PerTributarioExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _perTributarioService.GetPerTributarioAsync(new GetPerTributarioRequest { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un periodo tributario según los parámetros especificados (cliente, año y mes).
        /// </summary>
        /// <param name="request">
        /// Objeto que contiene los filtros de búsqueda:
        /// - <c>ClienteId</c>: Identificador del cliente asociado al periodo.
        /// - <c>Anio</c>: Año tributario.
        /// - <c>Mes</c>: Mes tributario.
        /// </param>
        /// <returns>
        /// Retorna un objeto <see cref="BaseResponseGeneric{T}"/> con la información del periodo tributario encontrado:
        /// - <c>Success = true</c> si se encontró el registro.
        /// - <c>Success = false</c> si no se encontró o ocurrió un error.
        /// </returns>
        [HttpGet("buscar")]
        public async Task<IActionResult> GetByPeriodo([FromQuery] GetPerTributarioByPeriodoRequest request)
        {
            try
            {
                var result = await _perTributarioService.GetPerTributarioByPeriodoAsync(request);

                // 📌 Devolver el código HTTP según el resultado del servicio
                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                // ⚠️ Manejo de errores inesperados
                return StatusCode(500, new BaseResponseGeneric<PerTributarioResponse>
                {
                    Success = false,
                    Message = $"Error inesperado: {ex.Message}",
                    ErrorCode = "EXCEPTION",
                    StatusCode = 500
                });
            }
        }

    }
}
