using Manager.Domain.Services.Interfaces;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenBaseController : ControllerBase
    {
        private readonly ITokenBaseService _tokenBaseService;
        private readonly ICuentaBaseSolService _cuentaBaseSolService;
        private readonly IClienteSolService _clienteSolService;

        public TokenBaseController(ITokenBaseService tokenBaseService, ICuentaBaseSolService cuentaBaseSolService, IClienteSolService clienteSolService)
        {
            _tokenBaseService = tokenBaseService;
            _cuentaBaseSolService = cuentaBaseSolService;
            _clienteSolService = clienteSolService;
        }

        [HttpGet("cuentaBaseSols/tokenBase-activo")]
        public async Task<IActionResult> GetOrGenerateActiveTokenBase()
        {
            // 1. Obtener cuentaBaseSol
            var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolFirstOrDefaultAsync();
            if (cuentaBaseSol == null)
                return NotFound("CuentaBaseSol no encontrado.");

            // 2. Obtener tokenBase actual de la BD
            var tokenBaseBD = await _tokenBaseService.GetTokenBaseAsync(new GetTokenBaseRequest { CuentaBaseSolId = cuentaBaseSol.Id });

            // Verifica si el tokenBase está activo (no vencido)
            bool tokenBaseActivo = tokenBaseBD != null &&
                               !tokenBaseBD.IsInactive && // ← ¡Asegúrate que este campo indique "activo"!
                               tokenBaseBD.FechaExpiracion > DateTime.UtcNow;

            if (tokenBaseActivo)
            {
                return Ok(new { accessToken = tokenBaseBD.AccessToken });
            }

            // 3. Solicita nuevo tokenBase a SUNAT
            var authResponse = await _clienteSolService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cuentaBaseSol.ClientId,
                ClientSecret = cuentaBaseSol.ClientSecret,
                Username = $"{cuentaBaseSol.Ruc}{cuentaBaseSol.Username}",
                Password = cuentaBaseSol.Password
            });

            if (!authResponse.Success)
                return StatusCode(502, "Error al obtener tokenBase desde SUNAT.");

            // 4. Calcula fechas
            DateTime ahora = DateTime.UtcNow;
            DateTime expiracion = ahora.AddSeconds(authResponse.Data.ExpiresIn);

            // 5. Guarda el tokenBase (nuevo o actualización)
            if (tokenBaseBD == null)
            {
                await _tokenBaseService.AddTokenBaseAsync(new AddTokenBaseRequest
                {
                    AccessToken = authResponse.Data.AccessToken,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,

                    CuentaBaseSolId = cuentaBaseSol.Id,
                });
            }
            else
            {
                await _tokenBaseService.EditTokenBaseAsync(new EditTokenBaseRequest
                {
                    Id = tokenBaseBD.Id,

                    AccessToken = authResponse.Data.AccessToken,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,

                    CuentaBaseSolId = cuentaBaseSol.Id
                });
            }

            // 6. Devuelve tokenBase actualizado
            return Ok(new { accessTokenBase = authResponse.Data.AccessToken });
        }
    }
}
