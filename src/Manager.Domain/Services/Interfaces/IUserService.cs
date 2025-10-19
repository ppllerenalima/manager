namespace Manager.Domain.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse> GetUserAsync(GetUserRequest request, CancellationToken cancellationToken = default);
        //Task<IEnumerable<UserResponse>> GetUserAsync(CancellationToken cancellationToken = default);
        Task<(IEnumerable<UserResponse> Items, int Total)> GetUsersAsync(string? search, int pageIndex, int pageSize);
        Task<TokenResponse> SignInAsync(SignInRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> SignUpAsync(SignUpRequest request, CancellationToken cancellationToken = default);
        Task<UserResponse> EditUserAsync(EditUserRequest request, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(Guid id, ChangePasswordUserRequest request, CancellationToken cancellationToken = default);
        Task<bool> ResetPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default);
    }
}