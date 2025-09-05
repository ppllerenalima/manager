namespace Manager.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [JsonException]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _userService.GetUserAsync();

            var totalGrupos = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.NombreCompleto)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<UserResponse>(pagination.PageIndex, pagination.PageSize, totalGrupos, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _userService.GetUserAsync(new GetUserRequest { Id = id });
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            var token = await _userService.SignInAsync(request);

            if (token == null) return BadRequest();
            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
            var user = await _userService.SignUpAsync(request);

            if (user == null) return BadRequest();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        [UserExists]
        public async Task<IActionResult> Update(Guid id, EditUserRequest request)
        {
            request.Id = id;
            var result = await _userService.EditUserAsync(request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}