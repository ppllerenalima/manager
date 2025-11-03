using Manager.Domain.Responses.ClienteResponses;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid? grupoId, [FromQuery] Guid? userId, [FromQuery] PaginationRequestModel request)
        {
            request ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var (itemsOnPage, total) = await _clienteService.GetClientesAsync(grupoId, userId, request.Search, request.PageIndex, request.PageSize);

            var model = new PaginatedResponseModel<ClienteResponse>(request.PageIndex, request.PageSize, total, itemsOnPage);

            return Ok(model);
        }

        [HttpGet("{Id:Guid}")]
        [ClienteExists]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var result = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = Id });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddClienteRequest request)
        {
            var result = await _clienteService.AddClienteAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, null);
        }

        [HttpPut("{id:guid}")]
        [ClienteExists]
        public async Task<IActionResult> Put(Guid id, EditClienteRequest request)
        {
            request.Id = id;
            var result = await _clienteService.EditClienteAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Activa el permiso de un cliente en el sistema.
        /// </summary>
        /// <param name="id">Identificador único del cliente.</param>
        /// <returns>
        /// Retorna un objeto <see cref="BaseResponse"/> con el resultado de la operación:
        /// - <c>Success = true</c> si se activó correctamente.
        /// - <c>Success = false</c> si ocurrió un error o el cliente no existe.
        /// </returns>
        [HttpPut("dar-permiso/{id:guid}")]
        [ClienteExists]
        public async Task<IActionResult> DarPermiso(Guid id)
        {
            var result = await _clienteService.DarPermisoAsync(id);
            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete("{id:guid}")]
        [ClienteExists]
        public async Task<IActionResult> Delete(Guid id)
        {
            var request = new DeleteClienteRequest { Id = id };
            await _clienteService.DeleteClienteAsync(request);
            return NoContent();
        }


    }
}
