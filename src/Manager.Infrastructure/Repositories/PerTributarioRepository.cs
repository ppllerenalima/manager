namespace Manager.Infrastructure.Repositories
{
    public class PerTributarioRepository : RepositoryBase<PerTributario>, IPerTributarioRepository
    {
        public PerTributarioRepository(ManagerContext context) : base(context)
        {
        }
    }
}