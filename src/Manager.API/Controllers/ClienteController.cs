using Manager.API.Filters;
using Manager.API.ResponseModels;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Responses;
using Manager.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get(
            [FromQuery] string? search = "",
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageIndex = 0,
            [FromQuery] Guid? grupoId = null)
        {
            // 1️⃣ Obtener lista filtrada del servicio
            var clientes = await _clienteService.GetClientesAsync(search ?? "");

            if (clientes == null)
                return NotFound("No se encontraron clientes.");

            // 2️⃣ Aplicar filtro por grupo (solo si se envía)
            if (grupoId.HasValue && grupoId.Value != Guid.Empty)
                clientes = clientes.Where(c => c.GrupoId == grupoId.Value).ToList();

            // 3️⃣ Total de registros después del filtro
            var total = clientes.Count();

            // 4️⃣ Paginación
            var items = clientes
                .OrderBy(c => c.Id)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            // 5️⃣ Construcción del modelo de respuesta
            var model = new PaginatedResponseModel<ClienteResponse>(
                pageIndex,
                pageSize,
                total,
                items
            );

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
