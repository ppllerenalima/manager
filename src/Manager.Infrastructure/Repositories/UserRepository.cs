using Manager.Domain.Entities;

namespace Manager.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<User> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            return result.Succeeded ? user : null;
        }

        public async Task<User> SignUpAsync(User user, string password, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return user; // el mismo objeto ya tiene Id y demás datos asignados
        }

        public async Task<bool> AddToRoleAsync(User user, string role, CancellationToken cancellationToken)
        {
            var result = await _userManager.AddToRoleAsync(user, role);

            return result.Succeeded;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserRoleAsync(User user, string newRole, CancellationToken cancellationToken)
        {
            // 1️⃣ Obtener los roles actuales
            var currentRoles = await _userManager.GetRolesAsync(user);

            // 2️⃣ Remover los roles existentes
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
                return false; // Maneja errores según tu lógica

            // 3️⃣ Agregar el nuevo rol
            var addResult = await _userManager.AddToRoleAsync(user, newRole);
            return addResult.Succeeded;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _userManager
                .Users
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item is null)
                throw new InvalidOperationException($"No se encontró el registro con id {id}");

            var result = await _userManager.DeleteAsync(item);
            return result.Succeeded;
        }

        public async Task<ICollection<User>> GetAsync(CancellationToken cancellationToken)
        {
            return await _userManager
                .Users
                .Include(u => u.Persona)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role) // 👈 Aquí traes el rol
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _userManager
                .Users
                .Include(z => z.Persona)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role) // 👈 Aquí traes el rol
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<string[]> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            var roles = await _userManager.GetRolesAsync(user); // devuelve IList<string>
            return roles.ToArray(); // convertimos a array si es necesario
        }

        // 🔒 Cambiar contraseña (requiere contraseña actual)
        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException($"No se encontró el usuario con id {userId}");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"No se pudo cambiar la contraseña: {errors}");
            }

            return true;
        }

        // 🔄 Resetear contraseña (sin necesidad de la actual)
        public async Task<bool> ResetPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException($"No se encontró el usuario con id {userId}");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"No se pudo resetear la contraseña: {errors}");
            }

            return true;
        }

    }
}