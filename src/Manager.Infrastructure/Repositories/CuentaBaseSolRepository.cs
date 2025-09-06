namespace Manager.Infrastructure.Repositories
{
    public class CuentaBaseSolRepository : RepositoryBase<CuentaBaseSOL>, ICuentaBaseSolRepository
    {
        public CuentaBaseSolRepository(ManagerContext context) : base(context)
        {

        }

        public async Task<CuentaBaseSOL?> GetFirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<CuentaBaseSOL>()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}