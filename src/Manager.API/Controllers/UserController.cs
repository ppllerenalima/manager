using Manager.API.Filters;
using Manager.Domain.Requests.User;
using Manager.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

            if (claim == null) return Unauthorized();

            var token = await _userService.GetUserAsync(new GetUserRequest { Email = claim.Value });
            return Ok(token);
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
            return CreatedAtAction(nameof(Get), new { }, null);
        }
    }
}