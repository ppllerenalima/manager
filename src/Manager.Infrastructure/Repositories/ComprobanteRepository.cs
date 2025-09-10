

namespace Manager.Infrastructure.Repositories
{
    public class ComprobanteRepository : RepositoryBase<Comprobante>, IComprobanteRepository
    {
        public ComprobanteRepository(ManagerContext context) : base(context)
        {
        }

        public async Task<ICollection<Comprobante>> AddAsync(IEnumerable<Comprobante> adds, CancellationToken cancellationToken)
        {
            await _context.Set<Comprobante>()
                .AddRangeAsync(adds, cancellationToken);

            return adds.ToList();
        }
    }
}