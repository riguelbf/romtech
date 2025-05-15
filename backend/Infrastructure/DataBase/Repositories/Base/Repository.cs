using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase.Repositories.Base
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;
        private readonly DbSet<T> _dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(DbContext context)
        {
            Context = context;
            _dbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets an entity by its primary key asynchronously.
        /// </summary>
        /// <param name="id">The entity primary key.</param>
        /// <returns>The entity if found; otherwise, throws InvalidOperationException.</returns>
        public async Task<T> GetByIdAsync(object id) => await _dbSet.FindAsync(id) ?? throw new InvalidOperationException();

        /// <summary>
        /// Gets all entities asynchronously.
        /// </summary>
        /// <returns>A list of all entities.</returns>
        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        /// <summary>
        /// Finds entities matching the given predicate asynchronously.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <returns>A list of matching entities.</returns>
        public async Task<IEnumerable<T>> FindAsync(Expression<System.Func<T, bool>> predicate)
            => await Task.FromResult(_dbSet.Where(predicate));

        /// <summary>
        /// Adds a new entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(T entity) => _dbSet.Update(entity);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        public void Remove(T entity) => _dbSet.Remove(entity);
    }
}
