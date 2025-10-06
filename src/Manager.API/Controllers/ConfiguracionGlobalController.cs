namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionGlobalController : ControllerBase
    {
        private readonly IConfiguracionGlobalService _configuracionGlobalService;

        public ConfiguracionGlobalController(IConfiguracionGlobalService configuracionGlobalService)
        {
            _configuracionGlobalService = configuracionGlobalService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _configuracionGlobalService.GetConfiguracionGlobalsAsync();

            var totalConfiguracionGlobals = result.Count();

            var itemsOnPage = result
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<ConfiguracionGlobalResponse>(pagination.PageIndex, pagination.PageSize, totalConfiguracionGlobals, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("FirstOrDefault")]
        public async Task<IActionResult> GetFirstOrDefault()
        {
            var result = await _configuracionGlobalService.GetConfiguracionGlobalFirstOrDefaultAsync();

            if (result is null)
                return NotFound(new BaseResponseGeneric<ConfiguracionGlobalResponse>
                {
                    Success = false,
                    ErrorMessage = "No se encontró ninguna cuenta base."
                });

            return Ok(new BaseResponseGeneric<ConfiguracionGlobalResponse>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _configuracionGlobalService.GetConfiguracionGlobalAsync(new GetConfiguracionGlobalRequest { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddConfiguracionGlobalRequest request)
        {
            var result = await _configuracionGlobalService.AddConfiguracionGlobalAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> Put(Guid id, EditConfiguracionGlobalRequest request)
        {
            request.Id = id;
            var result = await _configuracionGlobalService.EditConfiguracionGlobalAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteConfiguracionGlobalRequest { Id = id };
            await _configuracionGlobalService.DeleteConfiguracionGlobalAsync(request);
            return NoContent();
        }
    }
}
