using Manager.Domain.Entities.Enum;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Requests.PerTributario;
using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Requests.Ticket;
using Manager.Domain.Responses;
using Manager.Domain.Responses.ErroresResponses;
using Manager.Domain.Responses.PerTributarioResponses;
using Manager.Domain.Responses.TicketResponses;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SireComprasController : ControllerBase
    {
        private readonly ISireComprasService _sireComprasService;
        private readonly IClienteService _clienteSunatService;
        private readonly ITokenService _tokenService;
        private readonly ITicketService _ticketService;
        private readonly IPerTributarioService _perTributarioService;

        public SireComprasController(ISireComprasService sireComprasService, IClienteService clienteSunatService, ITokenService tokenService, ITicketService ticketService, IPerTributarioService perTributarioService)
        {
            _sireComprasService = sireComprasService;
            _clienteSunatService = clienteSunatService;
            _tokenService = tokenService;
            _ticketService = ticketService;
            _perTributarioService = perTributarioService;
        }

        [HttpGet("{Id:Guid}/token")]
        public async Task<IActionResult> GetToken(Guid Id)
        {
            var cliente = await _clienteSunatService.GetClienteAsync(new GetClienteRequest { Id = Id });

            var token = await _sireComprasService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cliente.ClientId,
                ClientSecret = cliente.ClientSecret,
                Username = $"{cliente.Ruc}{cliente.Username}",
                Password = cliente.Password
            });

            return Ok(token);
        }
    }
}
