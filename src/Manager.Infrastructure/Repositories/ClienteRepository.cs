using Manager.Domain.Entities;

namespace Manager.Infrastructure.Repositories
{
    public class ClienteRepository : RepositoryBase<Cliente>, IClienteRepository
    {
        public ClienteRepository(ManagerContext context) : base(context)
        {

        }
    }
}