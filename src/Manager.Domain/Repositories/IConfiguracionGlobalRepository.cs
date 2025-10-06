namespace Manager.Domain.Repositories
{
    public interface IConfiguracionGlobalRepository : IRepositoryBase<ConfiguracionGlobal>
    {
        Task<ConfiguracionGlobal?> GetFirstOrDefaultAsync(CancellationToken cancellationToken = default);

    }
}