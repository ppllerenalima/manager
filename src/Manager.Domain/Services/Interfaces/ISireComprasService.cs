using Manager.Domain.Requests.Sire.Compras;
using Manager.Domain.Responses;

namespace Manager.Domain.Services.Interfaces
{
    public interface ISireComprasService
    {
        Task<SunatAuthResponse> AccessTokenAsync(SunatAuthRequest request);
        Task<AceptarPropuestaResultado> AceptarPropuestaAsync(AceptarPropuestaRequest requestModel);
        Task<DescargarPropuestaResponse> DescargarPropuestaRCEAsync(DescargarPropuestaRequest request);
        Task<ConsultarEstadoTicketResponse> ConsultarEstadoTicketAsync(ConsultarEstadoTicketRequest request);
        Task<DescargarArchivoReporteResponse> DescargarArchivoReporteAsync(string token, DescargarArchivoReporteRequest request);
    }
}