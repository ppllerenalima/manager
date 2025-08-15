namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaService _grupoService;

        public PersonaController(IPersonaService grupoService)
        {
            _grupoService = grupoService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var result = await _grupoService.GetPersonasAsync();

            var totalPersonas = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.ApePaterno)
                .Skip(pageSize * pageIndex)
                .Take(pageSize);

            var model = new PaginatedResponseModel<PersonaResponse>(pageIndex, pageSize, totalPersonas, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{id:guid}")]
        [PersonaExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _grupoService.GetPersonaAsync(new GetPersonaRequest { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddPersonaRequest request)
        {
            var result = await _grupoService.AddPersonaAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [PersonaExists]
        public async Task<IActionResult> Put(Guid id, EditPersonaRequest request)
        {
            request.Id = id;
            var result = await _grupoService.EditPersonaAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [PersonaExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeletePersonaRequest { Id = id };
            await _grupoService.DeletePersonaAsync(request);
            return NoContent();
        }



    }
}
