using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Audacia.Seed.EntityFramework6.Extensions
{
    /// <summary>
	/// Seed entity provider that can return a new entity that is linked to an existing database entity.
    /// </summary>
    public abstract class SeedFromDatabase<T> : DbSeed<T>, ISetDbContext
        where T : class
    {
        /// <summary>
        /// Sets the internal database context.
        /// </summary>
        /// <param name="context">An initialised database context.</param>
        public void SetDbContext(DbContext context)
        {
            DbContext = context;
        }

        /// <summary>
        /// Gets the DbContext for the seed class.
        /// </summary>
        protected DbContext DbContext { get; private set; }

        /// <summary>
        /// Returns a query for DbEntities that have already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to return.</typeparam>
        protected IEnumerable<TEntity> DbEntity<TEntity>() where TEntity : class =>
            DbContext?.Set<TEntity>();

        /// <summary>
        /// Searches for a specific DbEntity that has already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        protected TEntity DbEntity<TEntity>(Func<TEntity, bool> selectorFunc) where TEntity : class =>
            DbContext?.Set<TEntity>().FirstOrDefault(selectorFunc);
    }
}
