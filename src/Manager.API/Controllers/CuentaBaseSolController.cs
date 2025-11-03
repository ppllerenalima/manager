namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentaBaseSolController : ControllerBase
    {
        private readonly ICuentaBaseSolService _cuentaBaseSolService;

        public CuentaBaseSolController(ICuentaBaseSolService cuentaBaseSolService)
        {
            _cuentaBaseSolService = cuentaBaseSolService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _cuentaBaseSolService.GetCuentaBaseSolsAsync();

            var totalCuentaBaseSols = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.Username)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<CuentaBaseSolResponse>(pagination.PageIndex, pagination.PageSize, totalCuentaBaseSols, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("FirstOrDefault")]
        public async Task<IActionResult> GetFirstOrDefault()
        {
            var result = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();

            if (result is null)
                return NotFound(new BaseResponseGeneric<CuentaBaseSolResponse>
                {
                    Success = false,
                    Message = "No se encontró ninguna cuenta base."
                });

            return Ok(new BaseResponseGeneric<CuentaBaseSolResponse>
            {
                Success = true,
                Data = result
            });
        }

        [HttpGet("{id:guid}")]
        [CuentaBaseSolExists]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _cuentaBaseSolService.GetCuentaBaseSolAsync(new GetCuentaBaseSolRequest { Id = id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddCuentaBaseSolRequest request)
        {
            var result = await _cuentaBaseSolService.AddCuentaBaseSolAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [CuentaBaseSolExists]
        public async Task<IActionResult> Put(Guid id, EditCuentaBaseSolRequest request)
        {
            request.Id = id;
            var result = await _cuentaBaseSolService.EditCuentaBaseSolAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id:guid}")]
        [CuentaBaseSolExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteCuentaBaseSolRequest { Id = id };
            await _cuentaBaseSolService.DeleteCuentaBaseSolAsync(request);
            return NoContent();
        }
    }
}
