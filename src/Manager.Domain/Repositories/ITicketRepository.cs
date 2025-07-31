namespace Manager.Domain.Repositories
{
    public interface ITicketRepository : IRepositoryBase<Ticket>
    {
        Task<Ticket?> GetAsync(Guid clienteId, string codProceso, string perTributario, CancellationToken cancellationToken = default);
    }
}