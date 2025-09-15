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

        [HttpGet("buscar")]
        public async Task<IActionResult> GetByPeriodo([FromQuery] GetPerTributarioByPeriodoRequest request)
        {
            var result = await _perTributarioService.GetPerTributarioByPeriodoAsync(request);
            return Ok(result);
        }
    }
}
