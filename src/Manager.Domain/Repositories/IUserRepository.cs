using Microsoft.AspNetCore.Identity;

namespace Manager.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<ICollection<User>> GetAsync(CancellationToken cancellationToken = default);
        Task<User> GetAsync(string id, CancellationToken cancellationToken);
    }
}