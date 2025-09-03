using Manager.API.RequestModels;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GrupoController : ControllerBase
    {
        private readonly IGrupoService _grupoService;

        public GrupoController(IGrupoService grupoService)
        {
            _grupoService = grupoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _grupoService.GetGruposAsync();

            var totalGrupos = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.Descripcion)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<GrupoResponse>(pagination.PageIndex, pagination.PageSize, totalGrupos, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{id:guid}")]
        [GrupoExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _grupoService.GetGrupoAsync(new GetGrupoRequest { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddGrupoRequest request)
        {
            var result = await _grupoService.AddGrupoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [GrupoExists]
        public async Task<IActionResult> Put(Guid id, EditGrupoRequest request)
        {
            request.Id = id;
            var result = await _grupoService.EditGrupoAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [GrupoExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteGrupoRequest { Id = id };
            await _grupoService.DeleteGrupoAsync(request);
            return NoContent();
        }
    }
}
