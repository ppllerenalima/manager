namespace Manager.Domain.Repositories
{
    public interface IRoleRepository
    {
        Task<Role> AddAsync(Role role, CancellationToken cancellationToken = default);
        Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<ICollection<Role>> GetAsync(CancellationToken cancellationToken = default);
        Task<Role> GetAsync(Guid id, CancellationToken cancellationToken);
    }
}