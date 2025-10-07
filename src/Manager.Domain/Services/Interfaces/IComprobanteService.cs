using Manager.Domain.Requests.Comprobante;

namespace Manager.Domain.Services.Interfaces
{
    public interface IComprobanteService
    {
        Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync(Guid perTributarioId, string search);
        Task<ICollection<Comrpobante_GlosaResponse>> ImportarGlosaAsync(Guid perTributarioId, string token, CancellationToken cancellationToken = default);
        Task<ComprobanteResponse> EditComprobanteAsync(EditComprobanteRequest request);
    }
}