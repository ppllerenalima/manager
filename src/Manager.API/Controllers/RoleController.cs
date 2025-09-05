using Manager.Domain.Requests.Role;

namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [JsonException]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _roleService.GetRoleAsync();

            var totalGrupos = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.Name)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<RoleResponse>(pagination.PageIndex, pagination.PageSize, totalGrupos, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var role = await _roleService.GetRoleAsync(new GetRoleRequest { Id = id });
            return Ok(role);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add(AddRoleRequest request)
        {
            var role = await _roleService.AddRoleAsync(request);

            if (role == null) return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        [RoleExists]
        public async Task<IActionResult> Update(Guid id, EditRoleRequest request)
        {
            request.Id = id;
            var result = await _roleService.EditRoleAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}