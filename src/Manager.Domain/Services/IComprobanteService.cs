namespace Manager.Domain.Services
{
    public interface IComprobanteService
    {
        Task<IEnumerable<ComprobanteResponse>> GetComprobantesAsync();
    }
}