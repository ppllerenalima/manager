namespace Manager.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken);
        Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ICollection<User>> GetAsync(CancellationToken cancellationToken = default);
        Task<User> GetAsync(Guid id, CancellationToken cancellationToken);
    }
}