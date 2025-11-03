namespace Manager.Domain.Services.Interfaces
{
    public interface IComprobanteService
    {
        Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync(Guid perTributarioId, string search);
        Task<BaseResponseGeneric<ICollection<Comprobante_GlosaResponse>>> ImportarGlosaAsync(
            Guid perTributarioId,
            string token,
            CancellationToken cancellationToken);
        Task<ComprobanteResponse> EditComprobanteAsync(EditComprobanteRequest request);
    }
}