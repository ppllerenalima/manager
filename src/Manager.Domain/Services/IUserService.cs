namespace Manager.Domain.Services
{
    public interface IUserService
    {
        //Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserResponse>> GetUserAsync(CancellationToken cancellationToken = default);
        //Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
        Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
    }
}