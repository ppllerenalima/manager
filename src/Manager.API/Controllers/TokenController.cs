namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IClienteService _clienteService;
        private readonly IClienteSolService _clienteSolService;

        public TokenController(ITokenService tokenService, IClienteService clienteService, IClienteSolService clienteSolService)
        {
            _tokenService = tokenService;
            _clienteService = clienteService;
            _clienteSolService = clienteSolService;
        }

        [HttpGet("clientes/{id:Guid}/token-activo")]
        public async Task<IActionResult> GetOrGenerateActiveToken(Guid id)
        {
            // 1. Obtener cliente
            var cliente = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = id });
            if (cliente == null)
                return NotFound("Cliente no encontrado.");

            // 2. Obtener token actual de la BD
            var tokenBD = await _tokenService.GetTokenAsync(new GetTokenRequest { ClienteId = id });

            // Verifica si el token está activo (no vencido)
            bool tokenActivo = tokenBD != null &&
                               !tokenBD.IsInactive && // ← ¡Asegúrate que este campo indique "activo"!
                               tokenBD.FechaExpiracion > DateTime.UtcNow;

            if (tokenActivo)
            {
                return Ok(new { accessToken = tokenBD.AccessToken });
            }

            // 3. Solicita nuevo token a SUNAT
            var authResponse = await _clienteSolService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cliente.ClientId,
                ClientSecret = cliente.ClientSecret,
                Username = $"{cliente.Ruc}{cliente.Username}",
                Password = cliente.Password
            });

            if (!authResponse.Success)
                return StatusCode(502, authResponse);

            // 4. Calcula fechas
            DateTime ahora = DateTime.UtcNow;
            DateTime expiracion = ahora.AddSeconds(authResponse.Data.Expires_in);

            // 5. Guarda el token (nuevo o actualización)
            if (tokenBD == null)
            {
                await _tokenService.AddTokenAsync(new AddTokenRequest
                {
                    AccessToken = authResponse.Data.Access_token,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,

                    ClienteId = cliente.Id,
                });
            }
            else
            {
                await _tokenService.EditTokenAsync(new EditTokenRequest
                {
                    Id = tokenBD.Id,

                    AccessToken = authResponse.Data.Access_token,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,

                    ClienteId = id
                });
            }

            // 6. Devuelve token actualizado
            return Ok(new { accessToken = authResponse.Data.Access_token });
        }

    }
}
