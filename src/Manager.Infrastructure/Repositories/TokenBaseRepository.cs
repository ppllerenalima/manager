namespace Manager.Infrastructure.Repositories
{
    public class TokenBaseRepository : ITokenBaseRepository
    {
        private readonly ManagerContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public TokenBaseRepository(ManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<TokenBase> GetAsync(Guid CuentaBaseSolId)
        {
            var tokenBaseCuentaBaseSol = await _context.TokenBases
                .AsNoTracking()
                .Where(x => x.CuentaBaseSolId == CuentaBaseSolId)
                .FirstOrDefaultAsync();

            if (tokenBaseCuentaBaseSol == null) return null;

            _context.Entry(tokenBaseCuentaBaseSol).State = EntityState.Detached;
            return tokenBaseCuentaBaseSol;
        }

        public TokenBase Add(TokenBase tokenBaseCuentaBaseSol)
        {
            return _context.TokenBases.Add(tokenBaseCuentaBaseSol).Entity;
        }

        public TokenBase Update(TokenBase tokenBaseCuentaBaseSol)
        {
            _context.Entry(tokenBaseCuentaBaseSol).State = EntityState.Modified;
            return tokenBaseCuentaBaseSol;
        }

        public TokenBase Delete(TokenBase tokenBaseCuentaBaseSol)
        {
            throw new NotImplementedException();
        }
    }
}