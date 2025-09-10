namespace Manager.Domain.Repositories
{
    public interface IComprobanteRepository : IRepositoryBase<Comprobante>
    {
        Task<ICollection<Comprobante>> AddAsync(IEnumerable<Comprobante> adds, CancellationToken cancellationToken);
    }
}