namespace Manager.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ManagerContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public ClienteRepository(ManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Cliente>> GetAsync(string search)
        {
            return await _context.Clientes
                .Where(z => z.Ruc.Contains(search) || z.Razonsocial.Contains(search))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Cliente> GetAsync(Guid Id)
        {
            var item = await _context.Clientes
                .AsNoTracking()
                .Where(x => x.Id == Id)
                .FirstOrDefaultAsync();

            if (item == null) return null;

            _context.Entry(item).State = EntityState.Detached;
            return item;
        }

        public Cliente Add(Cliente item)
        {
            return _context.Clientes.Add(item).Entity;
        }

        public Cliente Update(Cliente item)
        {
            _context.Entry(item).State = EntityState.Modified;
            return item;
        }

        public Cliente Delete(Cliente item)
        {
            throw new NotImplementedException();
        }
    }
}