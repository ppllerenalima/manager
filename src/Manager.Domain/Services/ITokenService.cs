using Manager.Domain.Requests.Token;
using Manager.Domain.Responses.TokenResponses;

namespace Manager.Domain.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetTokenAsync(GetTokenRequest request);
        Task<TokenResponse> AddTokenAsync(AddTokenRequest request);
        Task<TokenResponse> EditTokenAsync(EditTokenRequest request);
        Task<TokenResponse> DeleteTokenAsync(DeleteTokenRequest request);
    }
}