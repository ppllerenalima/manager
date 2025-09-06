namespace Manager.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Role> AddAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return role; // el mismo objeto ya tiene Id y demás datos asignados
        }

        public async Task<Role> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return role;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _roleManager
                .Roles
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item is null)
                throw new InvalidOperationException($"No se encontró el registro con id {id}");

            var result = await _roleManager.DeleteAsync(item);
            return result.Succeeded;
        }

        public async Task<ICollection<Role>> GetAsync(CancellationToken cancellationToken)
        {
            return await _roleManager
                .Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Role> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _roleManager
                .Roles
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}