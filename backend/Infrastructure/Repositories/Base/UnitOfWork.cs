using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Base
{
    public class UnitOfWork(DbContext context, Hashtable repositories) : IUnitOfWork
    {
        private bool _disposed;

        /// <summary>
        /// Gets the repository for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>The repository for the entity type.</returns>
        public IRepository<T>? Repository<T>() where T : class
        {
            var type = typeof(T).Name;

            if (repositories.ContainsKey(type)) return (IRepository<T>)repositories[type]!;
            var repoType = typeof(Repository<>);
            var repoInstance = Activator.CreateInstance(repoType.MakeGenericType(typeof(T)), context);
            repositories.Add(type, repoInstance);

            return (IRepository<T>)repositories[type]!;
        }

        /// <summary>
        /// Saves all changes made in this unit of work asynchronously.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

        /// <summary>
        /// Disposes the unit of work and releases resources.
        /// </summary>
        /// <param name="disposing">Whether managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Disposes the unit of work.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
