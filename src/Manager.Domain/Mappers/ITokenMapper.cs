using Manager.Domain.Entities;
using Manager.Domain.Requests.Token;
using Manager.Domain.Responses.TokenResponses;

namespace Manager.Domain.Mappers
{
    public interface ITokenMapper
    {
        Token Map(AddTokenRequest request);
        Token Map(EditTokenRequest request);
        TokenResponse Map(Token request);
    }
}