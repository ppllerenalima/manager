namespace Manager.Domain.Services
{
    public interface ICuentaBaseSolService
    {
        Task<IEnumerable<CuentaBaseSolResponse>> GetCuentaBaseSolsAsync();
        Task<CuentaBaseSolResponse> GetCuentaBaseSolFirstOrDefaultAsync();
        Task<CuentaBaseSolResponse> GetCuentaBaseSolAsync(GetCuentaBaseSolRequest request);
        Task<CuentaBaseSolResponse> AddCuentaBaseSolAsync(AddCuentaBaseSolRequest request);
        Task<CuentaBaseSolResponse> EditCuentaBaseSolAsync(EditCuentaBaseSolRequest request);
        Task<CuentaBaseSolResponse> DeleteCuentaBaseSolAsync(DeleteCuentaBaseSolRequest request);
    }
}