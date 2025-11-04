namespace Manager.Domain.Services.Interfaces
{
    public interface IPerTributarioService
    {
        Task<IEnumerable<PerTributarioResponse>> GetPerTributariosAsync();
        Task<PerTributarioResponse> GetPerTributarioAsync(GetPerTributarioRequest request);
        Task<BaseResponseGeneric<PerTributarioResponse>> GetPerTributarioByPeriodoAsync(GetPerTributarioByPeriodoRequest request);
        //Task<PerTributarioResponse> AddPerTributarioAsync(AddPerTributarioRequest request, CancellationToken cancellationToken = default);
        Task<BaseResponseGeneric<PerTributarioResponse>> AddPerTributarioAsync(AddPerTributarioRequest request, CancellationToken cancellationToken = default);
    }
}