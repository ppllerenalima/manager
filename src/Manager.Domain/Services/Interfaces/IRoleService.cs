namespace Manager.Domain.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleResponse> GetRoleAsync(GetRoleRequest request, CancellationToken cancellationToken = default);
        Task<IEnumerable<RoleResponse>> GetRoleAsync(CancellationToken cancellationToken = default);
        Task<RoleResponse> AddRoleAsync(AddRoleRequest request, CancellationToken cancellationToken = default);
        Task<RoleResponse> EditRoleAsync(EditRoleRequest request, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}