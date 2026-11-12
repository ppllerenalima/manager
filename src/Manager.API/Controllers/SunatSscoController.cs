using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SunatSscoController : ControllerBase
    {
        private readonly ISunatSscoRepository _repository;
        private readonly ISunatSscoService _sunatService;

        public SunatSscoController(ISunatSscoRepository repository, ISunatSscoService sunatService)
        {
            _repository = repository;
            _sunatService = sunatService;
        }

        // 1. Listar el padrón paginado y con buscador (RUC, Razón Social o Resolución)
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] string? filtro = null,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanoPagina = 20,
            CancellationToken cancellationToken = default)
        {
            var (items, total) = await _repository.ListarPaginadoAsync(filtro, pagina, tamanoPagina, cancellationToken);
            var ultimaAct = await _repository.ObtenerUltimaActualizacionAsync(cancellationToken);

            return Ok(new
            {
                items,
                total,
                pagina,
                tamanoPagina,
                ultimaActualizacion = ultimaAct?.TextoFechaSunat,
                fechaUltimaEjecucion = ultimaAct?.FechaEjecucion
            });
        }

        // 2. Botón manual en Angular: "Verificar actualización en SUNAT ahora"
        [HttpPost("sincronizar")]
        public async Task<IActionResult> SincronizarManual(CancellationToken cancellationToken)
        {
            var (actualizado, mensaje) = await _sunatService.SincronizarPadronAsync(cancellationToken);
            return Ok(new { actualizado, mensaje });
        }

        // 3. Cruce rápido con tus compras del SIRE: envías una lista de RUCs de proveedores y te devuelve los que están en SSCO
        [HttpPost("validar-rucs")]
        public async Task<IActionResult> ValidarRucs([FromBody] List<string> rucs, CancellationToken cancellationToken)
        {
            if (rucs == null || rucs.Count == 0)
                return Ok(Array.Empty<object>());

            var observados = await _repository.BuscarPorRucsAsync(rucs, cancellationToken);
            return Ok(observados);
        }
    }
}
