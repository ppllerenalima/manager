namespace Manager.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _userMapper;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IOptions<AuthenticationSettings> authenticationSettings, IMapper userMapper, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
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

        //public async Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken)
        //{
        //    var user = new User
        //    {
        //        Email = request.Email,
        //        UserName = request.Email,
        //    };

        //    bool isCreated = await _userRepository.SignUpAsync(user, request.Password, cancellationToken);

        //    return !isCreated ? null : new UserResponse { FirstName = request.FirstName, LastName = request.LastName, MiddleName = request.MiddleName, Email = request.Email };
        //}

        public async Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken)
        {
            bool isAuthenticated = await _userRepository.AuthenticateAsync(request.UserName, request.Password, cancellationToken);

            return !isAuthenticated ? null : new TokenResponse { AccessToken = GenerateSecurityToken(request) };
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