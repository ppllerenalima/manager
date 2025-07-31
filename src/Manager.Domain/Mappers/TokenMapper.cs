using Manager.Domain.Entities;
using Manager.Domain.Requests.Token;
using Manager.Domain.Responses.TokenResponses;

namespace Manager.Domain.Mappers
{
    public class TokenMapper : ITokenMapper
    {
        public Token Map(AddTokenRequest request)
        {
            if (request == null) return null;

            var token = new Token
            {
                AccessToken = request.AccessToken,
                FechaGeneracion = request.FechaGeneracion,
                FechaExpiracion = request.FechaExpiracion,
                IsInactive = request.IsInactive,

                ClienteId = request.ClienteId,
            };

            return token;
        }

        public Token Map(EditTokenRequest request)
        {
            if (request == null) return null;

            var tokenCliente = new Token
            {
                Id = request.Id,
                IsInactive = request.IsInactive,

                AccessToken = request.AccessToken,
                FechaGeneracion = request.FechaGeneracion,
                FechaExpiracion = request.FechaExpiracion,

                ClienteId = request.ClienteId
            };

            return tokenCliente;
        }

        public TokenResponse Map(Token request)
        {
            if (request == null) return null;

            var response = new TokenResponse
            {
                Id = request.Id,
                AccessToken = request.AccessToken,
                FechaGeneracion = request.FechaGeneracion,
                FechaExpiracion = request.FechaExpiracion,
                IsInactive = request.IsInactive,

                ClienteId = request.ClienteId,
            };

            return response;
        }
    }
}