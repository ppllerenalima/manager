using Azure.Core;
using Manager.API.Filters;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Requests.Token;
using Manager.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IClienteService _clienteService;
        private readonly ISireComprasService _sireComprasService;

        public TokenController(ITokenService tokenService, IClienteService clienteService, ISireComprasService sireComprasService)
        {
            _tokenService = tokenService;
            _clienteService = clienteService;
            _sireComprasService = sireComprasService;
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
            var authResponse = await _sireComprasService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cliente.ClientId,
                ClientSecret = cliente.ClientSecret,
                Username = $"{cliente.Ruc}{cliente.Username}",
                Password = cliente.Password
            });

            if (!authResponse.EsExito)
                return StatusCode(502, "Error al obtener token desde SUNAT.");

            // 4. Calcula fechas
            DateTime ahora = DateTime.UtcNow;
            DateTime expiracion = ahora.AddSeconds(authResponse.ExpiresIn);

            // 5. Guarda el token (nuevo o actualización)
            if (tokenBD == null)
            {
                await _tokenService.AddTokenAsync(new AddTokenRequest
                {
                    AccessToken = authResponse.AccessToken,
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

                    AccessToken = authResponse.AccessToken,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,

                    ClienteId = id
                });
            }

            // 6. Devuelve token actualizado
            return Ok(new { accessToken = authResponse.AccessToken });
        }

    }
}
