using Manager.API.RequestModels;
using Manager.Domain.Responses.ComprobanteResponses;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprobanteController : ControllerBase
    {
        private readonly IComprobanteService _comprobanteService;

        public ComprobanteController(IComprobanteService comprobanteService)
        {
            _comprobanteService = comprobanteService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginationRequestModel pagination)
        {
            pagination ??= new PaginationRequestModel(); // Si es null, crea con valores por defecto

            var result = await _comprobanteService.GetComprobantesAsync();

            var totalComprobantes = result.Count();

            var itemsOnPage = result
                .OrderBy(c => c.FechaEmision)
                .Skip(pagination.PageSize * pagination.PageIndex)
                .Take(pagination.PageSize);

            var model = new PaginatedResponseModel<ComprobanteResponse>(pagination.PageIndex, pagination.PageSize, totalComprobantes, itemsOnPage);

            return Ok(model);
        }
    }
}
