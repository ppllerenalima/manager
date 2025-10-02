using Manager.Domain.Requests.ConfiguracionGlobal;
using Manager.Domain.Responses.ConfiguracionGlobalResponses;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionGlobalController : ControllerBase
    {
        private readonly IConfiguracionGlobalService _ConfiguracionGlobalService;

        public ConfiguracionGlobalController(IConfiguracionGlobalService ConfiguracionGlobalService)
        {
            _ConfiguracionGlobalService = ConfiguracionGlobalService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _ConfiguracionGlobalService.GetConfiguracionGlobalsAsync();

            var totalConfiguracionGlobals = result.Count();

            var itemsOnPage = result
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<ConfiguracionGlobalResponse>(pagination.PageIndex, pagination.PageSize, totalConfiguracionGlobals, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _ConfiguracionGlobalService.GetConfiguracionGlobalAsync(new GetConfiguracionGlobalRequest { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddConfiguracionGlobalRequest request)
        {
            var result = await _ConfiguracionGlobalService.AddConfiguracionGlobalAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> Put(Guid id, EditConfiguracionGlobalRequest request)
        {
            request.Id = id;
            var result = await _ConfiguracionGlobalService.EditConfiguracionGlobalAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [ConfiguracionGlobalExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteConfiguracionGlobalRequest { Id = id };
            await _ConfiguracionGlobalService.DeleteConfiguracionGlobalAsync(request);
            return NoContent();
        }
    }
}
