namespace Manager.Infrastructure.Repositories
{
    public class GrupoRepository : RepositoryBase<Grupo>, IGrupoRepository
    {
        public GrupoRepository(ManagerContext context) : base(context)
        {
        }
    }
}