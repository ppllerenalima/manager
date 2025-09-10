namespace Manager.Domain.Services
{
    public interface IPerTributarioService
    {
        Task<IEnumerable<PerTributarioResponse>> GetPerTributariosAsync();
        Task<PerTributarioResponse> GetPerTributarioAsync(GetPerTributarioRequest request);
        Task<PerTributarioResponse> AddPerTributarioAsync(AddPerTributarioRequest request, CancellationToken cancellationToken = default);
    }
}