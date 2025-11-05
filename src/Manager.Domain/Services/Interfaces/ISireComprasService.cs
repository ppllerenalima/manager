namespace Manager.Domain.Services.Interfaces
{
    public interface ISireComprasService
    {
        Task<SunatAuthResponse> AccessTokenAsync(SunatAuthRequest request);
        Task<AceptarPropuestaResultado> AceptarPropuestaAsync(AceptarPropuestaRequest requestModel);
        Task<DescargarPropuestaResponse> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request);
        Task<BaseResponseGeneric<ConsultaEstadoTicketsResponse>> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<DescargarArchivoReporteResponse>> DescargarArchivoReporteAsync(string token, DescargarArchivoReporteRequest request);
    }
}