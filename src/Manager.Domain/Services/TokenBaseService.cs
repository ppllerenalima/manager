using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class TokenBaseService : ITokenBaseService
    {
        private readonly IMapper _mapper;
        private readonly ITokenBaseRepository _repo;
        private readonly ILogger<TokenBaseService> _logger;

        private readonly ICuentaBaseSolService _cuentaBaseSolService;
        private readonly ISireComprasService _sireComprasService;
        private readonly IClienteSolService _clienteSolService;

        public TokenBaseService(ITokenBaseRepository tokenCuentaBaseSolRepository, IMapper tokenMapper, ICuentaBaseSolService cuentaBaseSolService, ISireComprasService sireComprasService, IClienteSolService clienteSolService, ILogger<TokenBaseService> logger)
        {
            _repo = tokenCuentaBaseSolRepository;
            _mapper = tokenMapper;
            _logger = logger;

            _cuentaBaseSolService = cuentaBaseSolService;
            _sireComprasService = sireComprasService;
            _clienteSolService = clienteSolService;
        }

        public async Task<TokenBaseResponse> GetTokenBaseAsync(GetTokenBaseRequest request)
        {
            if (request?.CuentaBaseSolId == null) throw new ArgumentNullException();
            var entity = await _repo.GetAsync(request.CuentaBaseSolId);
            return _mapper.Map<TokenBaseResponse>(entity);
        }

        public async Task<TokenBaseResponse> AddTokenBaseAsync(AddTokenBaseRequest request)
        {
            var token = _mapper.Map<TokenBase>(request);

            var result = _repo.Add(token);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<TokenBaseResponse>(result);
        }

        public async Task<TokenBaseResponse> EditTokenBaseAsync(EditTokenBaseRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.CuentaBaseSolId);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.CuentaBaseSolId} is not present");

            var entity = _mapper.Map<TokenBase>(request);
            var result = _repo.Update(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map<TokenBaseResponse>(result);
        }

        public async Task<TokenBaseResponse> DeleteTokenBaseAsync(DeleteTokenBaseRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _repo.GetAsync(request.Id);
            result.IsInactive = true;

            _repo.Update(result);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<TokenBaseResponse>(result);
        }

        public async Task<TokenBaseResponse> GetOrGenerateActiveTokenBaseAsync(Guid cuentaBaseSolId)
        {
            // 1. Obtener cuentaBaseSol
            var cuentaBaseSol = await _cuentaBaseSolService.GetCuentaBaseSolAsync(new GetCuentaBaseSolRequest { Id = cuentaBaseSolId });
            if (cuentaBaseSol == null)
                throw new KeyNotFoundException("CuentaBaseSol no encontrado");

            // 2. Obtener token actual
            var tokenBD = await GetTokenBaseAsync(new GetTokenBaseRequest { CuentaBaseSolId = cuentaBaseSolId });

            // 3. Verificar token activo
            bool tokenActivo = tokenBD != null &&
                               !tokenBD.IsInactive &&
                               tokenBD.FechaExpiracion > DateTime.UtcNow;

            if (tokenActivo)
            {
                return tokenBD; // Devuelves el token actual
            }

            // 4. Solicitar nuevo token a SUNAT
            var authResponse = await _clienteSolService.AccessTokenAsync(new SunatAuthRequest
            {
                ClientId = cuentaBaseSol.ClientId,
                ClientSecret = cuentaBaseSol.ClientSecret,
                Username = $"{cuentaBaseSol.Ruc}{cuentaBaseSol.Username}",
                Password = cuentaBaseSol.Password
            });

            if (!authResponse.Success)
                throw new ApplicationException("Error al obtener token desde SUNAT");

            // 5. Guardar token en BD
            var ahora = DateTime.UtcNow;
            var expiracion = ahora.AddSeconds(authResponse.Data.ExpiresIn);

            if (tokenBD == null)
            {
                tokenBD = await AddTokenBaseAsync(new AddTokenBaseRequest
                {
                    AccessToken = authResponse.Data.AccessToken,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,
                    CuentaBaseSolId = cuentaBaseSol.Id,
                });
            }
            else
            {
                tokenBD = await EditTokenBaseAsync(new EditTokenBaseRequest
                {
                    Id = tokenBD.Id,
                    AccessToken = authResponse.Data.AccessToken,
                    FechaGeneracion = ahora,
                    FechaExpiracion = expiracion,
                    CuentaBaseSolId = cuentaBaseSolId
                });
            }

            return tokenBD;
        }
    }
}