namespace Manager.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ManagerContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public TokenRepository(ManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Token> GetAsync(Guid ClienteId)
        {
            var tokenCliente = await _context.Tokens
                .AsNoTracking()
                .Where(x => x.ClienteId == ClienteId)
                .FirstOrDefaultAsync();

            if (tokenCliente == null) return null;

            _context.Entry(tokenCliente).State = EntityState.Detached;
            return tokenCliente;
        }

        public Token Add(Token tokenCliente)
        {
            return _context.Tokens.Add(tokenCliente).Entity;
        }

        public Token Update(Token tokenCliente)
        {
            _context.Entry(tokenCliente).State = EntityState.Modified;
            return tokenCliente;
        }

        public Token Delete(Token tokenCliente)
        {
            throw new NotImplementedException();
        }
    }
}