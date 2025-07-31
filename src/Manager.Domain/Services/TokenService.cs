using Manager.Domain.Mappers;
using Manager.Domain.Repositories;
using Manager.Domain.Requests.Token;
using Manager.Domain.Responses.TokenResponses;
using Microsoft.Extensions.Logging;

namespace Manager.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenMapper _mapper;
        private readonly ITokenRepository _repo;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ITokenRepository tokenClienteRepository, ITokenMapper tokenClienteMapper, ILogger<TokenService> logger)
        {
            _repo = tokenClienteRepository;
            _mapper = tokenClienteMapper;
            _logger = logger;
        }

        public async Task<TokenResponse> GetTokenAsync(GetTokenRequest request)
        {
            if (request?.ClienteId == null) throw new ArgumentNullException();
            var entity = await _repo.GetAsync(request.ClienteId);
            return _mapper.Map(entity);
        }

        public async Task<TokenResponse> AddTokenAsync(AddTokenRequest request)
        {
            var tokenCliente = _mapper.Map(request);

            var result = _repo.Add(tokenCliente);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map(result);
        }

        public async Task<TokenResponse> EditTokenAsync(EditTokenRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.ClienteId);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.ClienteId} is not present");

            var entity = _mapper.Map(request);
            var result = _repo.Update(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map(result);
        }

        public async Task<TokenResponse> DeleteTokenAsync(DeleteTokenRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _repo.GetAsync(request.Id);
            result.IsInactive = true;

            _repo.Update(result);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map(result);
        }
    }
}