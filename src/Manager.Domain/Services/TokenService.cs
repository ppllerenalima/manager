using Manager.Domain.Services.Interfaces;

namespace Manager.Domain.Services
{
    public class TokenService : ITokenService
    {
        private readonly IMapper _mapper;
        private readonly ITokenRepository _repo;
        private readonly ILogger<TokenService> _logger;

        private readonly IClienteService _clienteService;
        private readonly ISireComprasService _sireComprasService;
        private readonly IClienteSolService _clienteSolService;

        public TokenService(ITokenRepository tokenClienteRepository, IMapper tokenMapper, IClienteService clienteService, ISireComprasService sireComprasService, IClienteSolService clienteSolService, ILogger<TokenService> logger)
        {
            _repo = tokenClienteRepository;
            _mapper = tokenMapper;
            _logger = logger;

            _clienteService = clienteService;
            _sireComprasService = sireComprasService;
            _clienteSolService = clienteSolService;
        }

        public async Task<TokenResponse> GetTokenAsync(GetTokenRequest request)
        {
            if (request?.ClienteId == null) throw new ArgumentNullException();
            var entity = await _repo.GetAsync(request.ClienteId);
            return _mapper.Map<TokenResponse>(entity);
        }

        public async Task<TokenResponse> AddTokenAsync(AddTokenRequest request)
        {
            var token = _mapper.Map<Token>(request);

            var result = _repo.Add(token);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<TokenResponse>(result);
        }

        public async Task<TokenResponse> EditTokenAsync(EditTokenRequest request)
        {
            var existingRecord = await _repo.GetAsync(request.ClienteId);

            if (existingRecord == null) throw new ArgumentException($"Entity with {request.ClienteId} is not present");

            var entity = _mapper.Map<Token>(request);
            var result = _repo.Update(entity);

            await _repo.UnitOfWork.SaveChangesAsync();
            return _mapper.Map<TokenResponse>(result);
        }

        public async Task<TokenResponse> DeleteTokenAsync(DeleteTokenRequest request)
        {
            if (request?.Id == null) throw new ArgumentNullException();

            var result = await _repo.GetAsync(request.Id);
            result.IsInactive = true;

            _repo.Update(result);
            await _repo.UnitOfWork.SaveChangesAsync();

            return _mapper.Map<TokenResponse>(result);
        }

        public async Task<BaseResponseGeneric<TokenResponse>> GetOrGenerateActiveTokenAsync(Guid clienteId, CancellationToken cancellationToken)
        {
            var response = new BaseResponseGeneric<TokenResponse>();

            try
            {
                // 1️⃣ Obtener cliente
                var cliente = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = clienteId });
                if (cliente == null)
                {
                    response.Success = false;
                    response.Message = $"No se encontró el cliente con ID {clienteId}.";
                    response.ErrorCode = "CLIENTE_NO_ENCONTRADO";
                    return response;
                }

                // 2️⃣ Consultar token existente
                var tokenBD = await GetTokenAsync(new GetTokenRequest { ClienteId = clienteId });

                // 3️⃣ Validar si el token sigue activo
                var tokenActivo = tokenBD is not null &&
                                  !tokenBD.IsInactive &&
                                  tokenBD.FechaExpiracion > DateTime.UtcNow;

                if (tokenActivo)
                {
                    response.Success = true;
                    response.Data = tokenBD;
                    response.Message = "Token activo recuperado correctamente.";
                    return response;
                }

                // 4️⃣ Solicitar nuevo token a SUNAT
                var authResponse = await _clienteSolService.AccessTokenAsync(new SunatAuthRequest
                {
                    GrantType = "password",
                    Scope = "https://api.sunat.gob.pe",
                    ClientId = cliente.ClientId,
                    ClientSecret = cliente.ClientSecret,
                    Username = $"{cliente.Ruc}{cliente.Username}",
                    Password = cliente.Password
                }, cancellationToken);

                if (!authResponse.Success || authResponse.Data == null || string.IsNullOrWhiteSpace(authResponse.Data.AccessToken))
                {
                    response.Success = false;
                    response.Message = authResponse.Message ?? "Error al obtener token desde SUNAT.";
                    response.ErrorCode = "SUNAT_ERROR";
                    response.StatusCode = authResponse.StatusCode;
                    response.Data = null;
                    return response;
                }

                // 5️⃣ Guardar o actualizar token en BD
                var ahora = DateTime.UtcNow;
                var expiracion = ahora.AddSeconds(authResponse.Data.ExpiresIn);

                if (tokenBD == null)
                {
                    tokenBD = await AddTokenAsync(new AddTokenRequest
                    {
                        AccessToken = authResponse.Data.AccessToken,
                        FechaGeneracion = ahora,
                        FechaExpiracion = expiracion,
                        ClienteId = cliente.Id
                    });
                }
                else
                {
                    tokenBD = await EditTokenAsync(new EditTokenRequest
                    {
                        Id = tokenBD.Id,
                        AccessToken = authResponse.Data.AccessToken,
                        FechaGeneracion = ahora,
                        FechaExpiracion = expiracion,
                        ClienteId = cliente.Id
                    });
                }

                // 6️⃣ Devolver resultado exitoso
                response.Success = true;
                response.Data = tokenBD;
                response.Message = "Token generado y actualizado correctamente.";
                response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Ocurrió un error al generar o recuperar el token.";
                response.ErrorCode = "EX";
                response.StatusCode = 500;
            }

            return response;
        }

        //public async Task<TokenResponse> GetOrGenerateActiveTokenAsync(Guid clienteId)
        //{
        //    // 1. Obtener cliente
        //    var cliente = await _clienteService.GetClienteAsync(new GetClienteRequest { Id = clienteId });
        //    if (cliente == null)
        //        throw new KeyNotFoundException("Cliente no encontrado");

        //    // 2. Obtener token actual
        //    var tokenBD = await GetTokenAsync(new GetTokenRequest { ClienteId = clienteId });

        //    // 3. Verificar token activo
        //    bool tokenActivo = tokenBD != null &&
        //                       !tokenBD.IsInactive &&
        //                       tokenBD.FechaExpiracion > DateTime.UtcNow;

        //    if (tokenActivo)
        //    {
        //        return tokenBD; // Devuelves el token actual
        //    }

        //    // 4. Solicitar nuevo token a SUNAT
        //    var authResponse = await _clienteSolService.AccessTokenAsync(new SunatAuthRequest
        //    {
        //        ClientId = cliente.ClientId,
        //        ClientSecret = cliente.ClientSecret,
        //        Username = $"{cliente.Ruc}{cliente.Username}",
        //        Password = cliente.Password
        //    });

        //    if (!authResponse.Success)
        //        throw new ApplicationException("Error al obtener token desde SUNAT");

        //    // 5. Guardar token en BD
        //    var ahora = DateTime.UtcNow;
        //    var expiracion = ahora.AddSeconds(authResponse.Data.ExpiresIn);

        //    if (tokenBD == null)
        //    {
        //        tokenBD = await AddTokenAsync(new AddTokenRequest
        //        {
        //            AccessToken = authResponse.Data.AccessToken,
        //            FechaGeneracion = ahora,
        //            FechaExpiracion = expiracion,
        //            ClienteId = cliente.Id,
        //        });
        //    }
        //    else
        //    {
        //        tokenBD = await EditTokenAsync(new EditTokenRequest
        //        {
        //            Id = tokenBD.Id,
        //            AccessToken = authResponse.Data.AccessToken,
        //            FechaGeneracion = ahora,
        //            FechaExpiracion = expiracion,
        //            ClienteId = clienteId
        //        });
        //    }

        //    return tokenBD;
        //}
    }
}