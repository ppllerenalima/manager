namespace Manager.Domain.Repositories
{
    public interface IClienteRepository : IRepository
    {
        Task<IEnumerable<Cliente>> GetAsync(string search);
        Task<Cliente> GetAsync(Guid Id);
        Cliente Add(Cliente item);
        Cliente Update(Cliente item);
        Cliente Delete(Cliente item);
    }
}