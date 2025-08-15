namespace Manager.Infrastructure.Repositories
{
    public class PersonaRepository : RepositoryBase<Persona>, IPersonaRepository
    {
        public PersonaRepository(ManagerContext context) : base(context)
        {
        }
    }
}