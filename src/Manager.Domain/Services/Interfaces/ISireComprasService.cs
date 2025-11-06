namespace Manager.Domain.Services.Interfaces
{
    public interface ISireComprasService
    {
        Task<BaseResponseGeneric<ExportacionComprobantePropuestaResponse>> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<ConsultaEstadoTicketsResponse>> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(string token, Guid clienteId, DescargarArchivoReporteRequest request, CancellationToken cancellationToken = default);
        //Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(string token, DescargarArchivoReporteRequest request, CancellationToken cancellationToken = default);
    }
}