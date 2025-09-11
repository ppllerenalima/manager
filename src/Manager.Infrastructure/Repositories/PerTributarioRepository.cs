namespace Manager.Infrastructure.Repositories
{
    public class PerTributarioRepository : RepositoryBase<PerTributario>, IPerTributarioRepository
    {
        public PerTributarioRepository(ManagerContext context) : base(context)
        {
        }

        public async Task<PerTributario> GetByPredicateAsync(Expression<Func<PerTributario, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Set<PerTributario>()
                .AsNoTracking()
                .FirstAsync(predicate, cancellationToken);
        }
    }
}