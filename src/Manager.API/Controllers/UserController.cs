using Manager.Domain.Services.Interfaces;
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel request)
        {
            request ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var (itemsOnPage, total) = await _userService.GetUsersAsync(request.Search, request.PageIndex, request.PageSize);

            var model = new PaginatedResponseModel<UserResponse>(request.PageIndex, request.PageSize, total, itemsOnPage);

            return Ok(model);
        }

        [AllowAnonymous]
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

        [HttpPost("{id:guid}/change-password")]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordUserRequest request, CancellationToken cancellationToken)
        {
            
            try
            {
                // Obtener el nombre de usuario del token actual
                var userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                // (Opcional) Validar que el usuario autenticado sea el mismo que intenta cambiar su contraseña
                // Si usás GUIDs en el token, podrías comparar ClaimTypes.NameIdentifier en lugar de Name
                // var userIdFromToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                // if (userIdFromToken != id.ToString()) return Forbid();

                var result = await _userService.ChangePasswordAsync(id, request, cancellationToken);

                if (!result)
                    return BadRequest(new { success = false, message = "No se pudo cambiar la contraseña." });

                return Ok(new { success = true, message = "Contraseña actualizada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Error al cambiar la contraseña: {ex.Message}" });
            }
        }

        [HttpPost("{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener el nombre de usuario del token actual
                var userName = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;

                // (Opcional) Validar que el usuario autenticado sea el mismo que intenta cambiar su contraseña
                // var userIdFromToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                // if (userIdFromToken != id.ToString()) return Forbid();

                // Restablecer la contraseña con la clave por defecto
                const string defaultPassword = "Aa123*";
                var result = await _userService.ResetPasswordAsync(id, defaultPassword, cancellationToken);

                if (!result)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No se pudo restablecer la contraseña. Intente nuevamente o contacte al administrador del sistema."
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "La contraseña se restableció correctamente con la clave por defecto."
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = $"No se encontró el usuario especificado: {ex.Message}"
                });
            }
            catch (Exception ex)
            {
                // Loguear la excepción si existe un logger configurado
                // _logger.LogError(ex, "Error al restablecer contraseña del usuario {UserId}", id);

                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    success = false,
                    message = $"Ocurrió un error inesperado al restablecer la contraseña: {ex.Message}"
                });
            }
        }
    }
}