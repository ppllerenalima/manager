namespace Manager.Infrastructure.Repositories
{
    public class ConfiguracionGlobalRepository : RepositoryBase<ConfiguracionGlobal>, IConfiguracionGlobalRepository
    {
        public ConfiguracionGlobalRepository(ManagerContext context) : base(context)
        {

        }

        public async Task<ConfiguracionGlobal?> GetFirstOrDefaultAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<ConfiguracionGlobal>()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}