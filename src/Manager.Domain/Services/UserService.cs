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

        //public async Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken)
        //{
        //    var response = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        //    return new UserResponse
        //    {
        //        Email = response.Email
        //    };
        //}

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

        //public async Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        //{
        //    var persona = new Persona
        //    {
        //        ApePaterno = request.ApePaterno,
        //        ApeMaterno = request.ApeMaterno,
        //        Nombre = request.Nombre,
        //        IsInactive = request.IsInactive
        //    };

        //    await _personaRepository.AddAsync(persona, cancellationToken);
        //    await _personaRepository.UnitOfWork.SaveChangesAsync();

        //    var user = new User
        //    {
        //        UserName = request.UserName,
        //        Email = request.Email,
        //        EmailConfirmed = true,
        //        PersonaId = persona.Id
        //    };

        //    if (string.IsNullOrEmpty(request.Password))
        //        request.Password = request.UserName;

        //    bool isCreated = await _userRepository.SignUpAsync(user, request.Password, cancellationToken);

        //    return !isCreated ? null : new UserResponse { ApePaterno = request.ApePaterno, ApeMaterno = request.ApeMaterno, Nombre = request.Nombre, Email = request.Email };
        //}

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