using Manager.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<bool> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            return result.Succeeded;
        }

        public async Task<bool> SignUpAsync(User user, string password, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
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
                .Include(z=> z.Persona)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<User> GetAsync(string id, CancellationToken cancellationToken)
        {
            return await _userManager
                .Users
                .Include(z=> z.Persona)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }
    }
}