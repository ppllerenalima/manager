namespace Manager.Infrastructure.Repositories
{
    public class CuentaBaseSolRepository : RepositoryBase<CuentaBaseSOL>, ICuentaBaseSolRepository
    {
        public CuentaBaseSolRepository(ManagerContext context) : base(context)
        {
        }
    }
}