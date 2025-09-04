namespace Manager.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _userMapper;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepository _userRepository;
        private readonly IPersonaRepository _personaRepository;

        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IPersonaRepository personaRepository, IOptions<AuthenticationSettings> authenticationSettings, IMapper userMapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _personaRepository = personaRepository;
            _authenticationSettings = authenticationSettings.Value;
            _userMapper = userMapper;
            _logger = logger;
        }

        public async Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAsync(request.Id, cancellationToken);
            return _userMapper.Map<UserResponse>(result);
        }

        public async Task<IEnumerable<UserResponse>> GetUserAsync(CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetAsync(cancellationToken);

            //return _userMapper.Map<ICollection<UserResponse>>(result);

            return result
                .Select(x => _userMapper.Map<UserResponse>(x));
        }

        public async Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userRepository.AuthenticateAsync(request.UserName, request.Password, cancellationToken);

            return !isAuthenticated ? null : new TokenResponse { AccessToken = GenerateSecurityToken(request) };
        }

        public async Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        {
            // iniciamos una transacción en el UnitOfWork
            using var transaction = await _personaRepository.UnitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var persona = new Persona
                {
                    ApePaterno = request.ApePaterno,
                    ApeMaterno = request.ApeMaterno,
                    Nombre = request.Nombre,
                    IsInactive = request.IsInactive
                };

                await _personaRepository.AddAsync(persona, cancellationToken);
                await _personaRepository.UnitOfWork.SaveChangesAsync();

                var user = new User
                {
                    UserName = request.UserName,
                    Email = request.Email,
                    EmailConfirmed = true,
                    PersonaId = persona.Id
                };

                var password = "Aa123*";

                bool isCreated = await _userRepository.SignUpAsync(user, password, cancellationToken);

                if (!isCreated)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return null;
                }

                // confirmamos la transacción
                await transaction.CommitAsync(cancellationToken);

                return new UserResponse
                {
                    ApePaterno = request.ApePaterno,
                    ApeMaterno = request.ApeMaterno,
                    Nombre = request.Nombre,
                    Email = request.Email
                };
            }
            catch
            {
                // si ocurre cualquier excepción, revertimos todo
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<UserResponse> EditUserAsync(EditUserRequest request, CancellationToken cancellationToken)
        {
            // Abrimos una transacción en el UnitOfWork
            await using var transaction = await _personaRepository.UnitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 1. Obtener el registro existente
                var existingRecord = await _userRepository.GetAsync(request.Id, cancellationToken);
                if (existingRecord == null)
                    throw new ArgumentException($"No se encontró el usuario con Id {request.Id}");

                // 2. Actualizar Persona
                var existingPersona = existingRecord.Persona;
                if (existingPersona == null)
                    throw new ArgumentException($"No se encontró la persona vinculada al usuario {request.Id}");

                _userMapper.Map(request, existingPersona); // mapea sobre la misma instancia

                // 3. Actualizar User (sobre la misma entidad ya trackeada)
                _userMapper.Map(request, existingRecord);

                // 4. Guardar cambios de ambas entidades en una sola transacción
                await _personaRepository.UnitOfWork.SaveChangesAsync(cancellationToken);

                // 5. Confirmar transacción
                await transaction.CommitAsync(cancellationToken);

                // 6. Retornar DTO
                return _userMapper.Map<UserResponse>(existingRecord);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteAsync(string id, CancellationToken cancellationToken)
        {
            // 1. Obtener el registro existente
            var existingRecord = await _userRepository.GetAsync(id, cancellationToken);

            // 1. Iniciar una transacción en el contexto principal
            await using var transaction = await _personaRepository.UnitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 3. Eliminar usuario
                var result = await _userRepository.DeleteAsync(id);
                if (!result)
                {
                    throw new InvalidOperationException($"Error al eliminar usuario.");
                }

                // 4. Eliminar persona asociada (si existe)
                await _personaRepository.DeleteAsync(existingRecord.PersonaId, cancellationToken);

                // 5. Confirmar transacción
                await _personaRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }


        private string GenerateSecurityToken(SignInRequest request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authenticationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, request.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(_authenticationSettings.ExpirationDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}