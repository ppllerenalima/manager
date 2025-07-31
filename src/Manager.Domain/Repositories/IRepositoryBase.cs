namespace Manager.Domain.Repositories
{
    public interface IRepositoryBase<TEntity> : IRepository where TEntity : EntityBase
    //public interface IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        Task<ICollection<TEntity>> GetAsync(CancellationToken cancellationToken = default);

        Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);

        Task<ICollection<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<ICollection<TEntity>> GetAsync<Tkey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, Tkey>> orderBy,
            CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        //Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default); // Opcional
    }
}