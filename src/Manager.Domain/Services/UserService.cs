using Manager.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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

            return result
                .Select(x => _userMapper.Map<UserResponse>(x));
        }

        public async Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.AuthenticateAsync(request.UserName, request.Password, cancellationToken);
            if (user == null) return null;

            var persona = await _personaRepository.GetAsync(user.PersonaId, cancellationToken);

            var roles = await _userRepository.GetRolesAsync(user);

            return new TokenResponse
            {
                Id = new Guid(),
                AccessToken = GenerateSecurityToken(user),
                FechaGeneracion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddDays(_authenticationSettings.ExpirationDays),
                IsInactive = false,
                UserId = user.Id,

                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{persona.ApePaterno} {persona.ApeMaterno}, {persona.Nombre}",
                Role = roles.FirstOrDefault(), // tomamos el primer rol
            };
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

                var createdRole = await _userRepository.SignUpAsync(user, password, cancellationToken);

                bool isCreated = await _userRepository.AddToRoleAsync(user, request.Role);

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

                // 5️⃣ Actualizar rol del usuario si viene en el request
                if (!string.IsNullOrWhiteSpace(request.Role))
                {
                    var roleUpdated = await _userRepository.UpdateUserRoleAsync(existingRecord, request.Role, cancellationToken);
                    if (!roleUpdated)
                        throw new InvalidOperationException($"No se pudo actualizar el rol del usuario {request.Id}");
                }

                // 6. Confirmar transacción
                await transaction.CommitAsync(cancellationToken);

                // 7. Retornar DTO
                return _userMapper.Map<UserResponse>(existingRecord);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
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

        private string GenerateSecurityToken(User request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_authenticationSettings.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, request.Id.ToString()), // id del usuario
                    new Claim(ClaimTypes.Name, request.UserName),                // username real
                    new Claim(ClaimTypes.Email, request.Email),                  // email real
                    new Claim(ClaimTypes.Role, "Admin")                       // rol(es)
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