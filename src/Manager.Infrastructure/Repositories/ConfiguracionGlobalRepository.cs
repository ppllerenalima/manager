namespace Manager.Infrastructure.Repositories
{
    public class ConfiguracionGlobalRepository : RepositoryBase<ConfiguracionGlobal>, IConfiguracionGlobalRepository
    {
        public ConfiguracionGlobalRepository(ManagerContext context) : base(context)
        {
        }
    }
}