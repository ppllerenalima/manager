namespace Manager.Domain.Repositories
{
    public interface ICuentaBaseSolRepository : IRepositoryBase<CuentaBaseSOL>
    {
        Task<CuentaBaseSOL?> GetFirstOrDefaultAsync(CancellationToken cancellationToken = default);
    }
}