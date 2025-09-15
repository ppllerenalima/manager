using Manager.Domain.Requests.Ticket;
using Manager.Domain.Services.Interfaces;

namespace Manager.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IClienteService _clienteService;
        private readonly ISireComprasService _sireComprasService;

        public TicketController(ITicketService ticketService, IClienteService clienteService, ISireComprasService sireComprasService)
        {
            _ticketService = ticketService;
            _clienteService = clienteService;
            _sireComprasService = sireComprasService;
        }

        [HttpPost("get-or-generate-active-ticket")]
        public async Task<IActionResult> GetOrGenerateActiveTicketAsync([FromBody] GetTicketRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cliente = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = request.clienteId });
            if (cliente is null)
                return NotFound($"Cliente con ID {request.clienteId} no encontrado.");

            //var ticket = await _ticketService.GetTicketAsync(request);
            //if (ticket is not null)
            //    return await ProcesarTicketExistenteAsync(ticket, request);

            return null;
        }

        //private async Task<IActionResult> ProcesarTicketExistenteAsync(TicketResponse ticket, GetTicketRequest request)
        //{
        //    if (ticket.CodEstadoEnvio.Equals("06"))
        //        return Ok(ticket);

        //    var estadoTicket = await _sireComprasService.ConsultarEstadoTicketAsync(new ConsultarEstadoTicketRequest
        //    {
        //        AccessToken = request.AccessToken,
        //        PerIni = request.perTributario,
        //        PerFin = request.perTributario,
        //        Page = request.Page,
        //        PerPage = request.PerPage,
        //        NumTicket = ticket.NumTicket
        //    });

        //    if (estadoTicket == null || !estadoTicket.EsExito || estadoTicket.Result == null || estadoTicket.Result.Registros == null || !estadoTicket.Result.Registros.Any())
        //    {
        //        var mensajeError = estadoTicket?.Errores != null && estadoTicket.Errores.Any()
        //            ? string.Join(" | ", estadoTicket.Errores.Select(e => $"[{e.Cod}] {e.Msg}"))
        //            : "Error al consultar ticket en SUNAT.";

        //        return StatusCode(502, mensajeError);
        //    }

        //    var registro = estadoTicket.Result.Registros.First();
        //    var detalle = registro.DetalleTicket;
        //    var archivo = registro.ArchivoReporte?.FirstOrDefault();
        //    var estado = detalle?.CodEstadoEnvio;

        //    //Actualizar ticket en BD
        //    var updatedTicket = await _ticketService.EditTicketAsync(new EditTicketRequest
        //    {
        //        Id = ticket.Id,

        //        CodProceso = registro.CodProceso,
        //        CodEstadoProceso = registro.CodEstadoProceso,
        //        DesProceso = registro.DesProceso,
        //        PerTributario = registro.PerTributario,

        //        NumTicket = registro.NumTicket,
        //        FecCargaImportacion = detalle?.FecCargaImportacion,
        //        HoraCargaImportacion = detalle?.HoraCargaImportacion,
        //        CodEstadoEnvio = estado,
        //        DesEstadoEnvio = detalle?.DesEstadoEnvio,

        //        CodTipoAchivoReporte = archivo?.CodTipoAchivoReporte,
        //        NomArchivoReporte = archivo?.NomArchivoReporte,

        //        ClienteId = request.clienteId
        //    });

        //    if (estado == "06")
        //    {
        //        return Ok(updatedTicket);
        //    }

        //    if (estado == "05")
        //    {
        //        return StatusCode(409, $"El ticket está en un estado {estado}.");
        //    }

        //    return StatusCode(409, $"El ticket está en un estado no manejado: {estado}");
        //}

        //private async Task<IActionResult> GenerarNuevoTicketAsync(GetTicketRequest request)
        //{
        //    var generarTicket = await _sireComprasService.DescargarPropuestaRCEAsync(new DescargarPropuestaRequest
        //    {
        //        Token = request.AccessToken,
        //        PerTributario = request.perTributario
        //    });

        //    if (generarTicket == null || string.IsNullOrEmpty(generarTicket.Result.NumTicket))
        //        return StatusCode(500, "No se pudo generar un nuevo ticket.");

        //    var estadoTicket = await _sireComprasService.ConsultarEstadoTicketAsync(new ConsultarEstadoTicketRequest
        //    {
        //        AccessToken = request.AccessToken,
        //        PerIni = request.perTributario,
        //        PerFin = request.perTributario,
        //        Page = request.Page,
        //        PerPage = request.PerPage,
        //        NumTicket = generarTicket.Result.NumTicket
        //    });

        //    // 1. Validación de respuesta
        //    if (estadoTicket?.EsExito != true ||
        //        estadoTicket.Result?.Registros == null ||
        //        !estadoTicket.Result.Registros.Any())
        //    {
        //        return StatusCode(502, "No se pudo consultar el estado del ticket en SUNAT.");
        //    }

        //    var registro = estadoTicket.Result.Registros.First();
        //    var detalle = registro.DetalleTicket;
        //    var archivo = registro.ArchivoReporte?.FirstOrDefault();
        //    var estadoEnvio = detalle?.CodEstadoEnvio;

        //    // 2. Creo ticket en BD
        //    var nuevoTicket = await _ticketService.AddTicketAsync(new AddTicketRequest
        //    {
        //        CodProceso = registro.CodProceso,
        //        CodEstadoProceso = registro.CodEstadoProceso,
        //        DesProceso = registro.DesProceso,
        //        PerTributario = registro.PerTributario,

        //        NumTicket = registro.NumTicket,
        //        FecCargaImportacion = detalle?.FecCargaImportacion,
        //        HoraCargaImportacion = detalle?.HoraCargaImportacion,
        //        CodEstadoEnvio = estadoEnvio,
        //        DesEstadoEnvio = detalle?.DesEstadoEnvio,

        //        CodTipoAchivoReporte = archivo?.CodTipoAchivoReporte,
        //        NomArchivoReporte = archivo?.NomArchivoReporte,

        //        ClienteId = request.clienteId,
        //    });

        //    return Ok(nuevoTicket);
        //}
    }
}
