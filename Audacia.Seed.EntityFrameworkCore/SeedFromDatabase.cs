using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore
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
        protected IQueryable<TEntity> DbEntity<TEntity>() where TEntity : class =>
            DbContext?.Set<TEntity>();

        /// <summary>
        /// Searches for a specific DbEntity that has already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        protected TEntity DbEntity<TEntity>(Expression<Func<TEntity, bool>> selectorExpr) where TEntity : class =>
            DbContext?.Set<TEntity>().FirstOrDefault(selectorExpr);

        /// <summary>
        /// Returns a query for DbEntities that have already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        /// <typeparam name="TEntity">The type of entities to return.</typeparam>
        protected IQueryable<TEntity> DbView<TEntity>() where TEntity : class =>
            DbContext?.Query<TEntity>();

        /// <summary>
        /// Searches for a specific DbEntity that has already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        protected TEntity DbView<TEntity>(Expression<Func<TEntity, bool>> selectorExpr) where TEntity : class =>
            DbContext?.Query<TEntity>().FirstOrDefault(selectorExpr);
    }
}
