namespace Manager.Domain.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(GetTokenRequest request);
        Task<TokenResponse> AddTokenAsync(AddTokenRequest request);
        Task<TokenResponse> EditTokenAsync(EditTokenRequest request);
        Task<TokenResponse> DeleteTokenAsync(DeleteTokenRequest request);
        Task<BaseResponseGeneric<TokenResponse>> GetOrGenerateActiveTokenAsync(Guid clienteId, CancellationToken cancellationToken = default);
    }
}