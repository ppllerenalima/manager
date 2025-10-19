namespace Manager.Infrastructure.Repositories
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
    {
        protected readonly ManagerContext _context;
        public IUnitOfWork UnitOfWork => _context;

        public RepositoryBase(ManagerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public virtual async Task<ICollection<TEntity>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<ICollection<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ICollection<TEntity>> GetAsync<Tkey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, Tkey>> orderBy,
            CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>()
                .Where(predicate)
                .OrderBy(orderBy)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public IQueryable<TEntity> Get<TKey>(
            Expression<Func<TEntity, bool>>? predicate,
            Expression<Func<TEntity, TKey>>? orderBy,
            bool descending)
        {
            var query = _context.Set<TEntity>().AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = descending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);

            return query;
        }


        public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public virtual Task<TEntity> UpdateAsync(TEntity entity)
        {
            var local = _context.Set<TEntity>()
                .Local
                .FirstOrDefault(e => e.Id == entity.Id);

            if (local != null)
            {
                _context.Entry(local).State = EntityState.Detached; // 👈 Importante
            }

            _context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var item = await _context.Set<TEntity>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (item is not null)
                _context.Set<TEntity>().Remove(item);
            else
                throw new InvalidOperationException($"No se encontró el registro con id {id}");
        }
    }
}
