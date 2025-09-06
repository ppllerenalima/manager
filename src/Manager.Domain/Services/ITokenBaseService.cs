namespace Manager.Domain.Services
{
    public interface ITokenBaseService
    {
        Task<TokenBaseResponse> GetTokenBaseAsync(GetTokenBaseRequest request);
        Task<TokenBaseResponse> AddTokenBaseAsync(AddTokenBaseRequest request);
        Task<TokenBaseResponse> EditTokenBaseAsync(EditTokenBaseRequest request);
        Task<TokenBaseResponse> DeleteTokenBaseAsync(DeleteTokenBaseRequest request);
        Task<TokenBaseResponse> GetOrGenerateActiveTokenBaseAsync(Guid cuentaBaseSolId);
    }
}