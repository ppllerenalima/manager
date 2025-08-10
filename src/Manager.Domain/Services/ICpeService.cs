using Manager.Domain.Requests.Cpe;
using Manager.Domain.Responses.CpeResponses;

namespace Manager.Domain.Services
{
    public interface ICpeService
    {
        Task<StatusResponse> StatusCdrAsync(ConsultarCpeRequest request);
        Task<ConsultaCpeUnificadoResponse> ControlCpeConsultaXmlAsync(string token, ConsultaCpeComprobanteRequest request);
        Task<ConsultaCpeUnificadoResponse> ConsultaCpeComprobanteAsync(string token, ConsultaCpeComprobanteRequest request);
    }
}
