namespace Manager.Domain.Repositories
{
    public interface IPerTributarioRepository : IRepositoryBase<PerTributario>
    {
        Task<PerTributario> GetByPredicateAsync(Expression<Func<PerTributario, bool>> predicate, CancellationToken cancellationToken = default);
    }
}