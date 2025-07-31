using Manager.API.Filters;
using Manager.API.ResponseModels;
using Manager.Domain.Requests.Cliente;
using Manager.Domain.Responses;
using Manager.Domain.Services;
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
            [FromQuery] int pageIndex = 0)
        {
            // 1. Obtiene todos los clientes filtrados (si corresponde)
            var result = await _clienteService.GetClientesAsync(search ?? "");

            // 2. Total de registros filtrados
            var totalClientes = result.Count();

            // 3. Paginación
            var clientesOnPage = result
                .OrderBy(c => c.Id)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToList();

            // 4. Construye el modelo paginado
            var model = new PaginatedResponseModel<ClienteResponse>(
                pageIndex,
                pageSize,
                totalClientes,
                clientesOnPage
            );

            // 5. Devuelve respuesta HTTP 200 con el modelo
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
