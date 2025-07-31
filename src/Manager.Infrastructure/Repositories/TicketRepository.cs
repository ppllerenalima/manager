using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Repositories
{
    public class TicketRepository : RepositoryBase<Ticket>, ITicketRepository
    {
        public TicketRepository(ManagerContext context) : base(context)
        {
        }

        // Sobreescribimos solo este método
        public async Task<Ticket?> GetAsync(Guid clienteId, string codProceso, string perTributario, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Ticket>()
                .FirstOrDefaultAsync(z => z.ClienteId == clienteId && z.CodProceso.Equals(codProceso) && z.PerTributario.Equals(perTributario), cancellationToken);
        }
    }
}