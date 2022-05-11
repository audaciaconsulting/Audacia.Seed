﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Audacia.Seed.EntityFramework6.Extensions
{
    /// <summary>
	/// Seed entity provider that can return a new entity that is linked to an existing database entity.
    /// </summary>
    /// <typeparam name="T">The type of entity to be seeded with a link to a pre-existing entity in the database.</typeparam>
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
        /// <returns>A query for DbEntities of the specified type.</returns>
        protected IQueryable<TEntity> DbEntities<TEntity>() where TEntity : class =>
            DbContext?.Set<TEntity>();

        /// <summary>
        /// Searches for a specific DbEntity that has already been seeded to the DbContext.
        /// Will return null when initialised by AutoFixture.
        /// </summary>
        /// <typeparam name="TEntity">The type of entity to return.</typeparam>
        /// <param name="selectorFunc">Function providing criteria for selecting the entity.</param>
        /// <returns>The first pre-existing entity meeting the selector function's criteria, or null if none found.</returns>
        protected TEntity DbEntity<TEntity>(Expression<Func<TEntity, bool>> selectorFunc) where TEntity : class =>
            DbContext?.Set<TEntity>().FirstOrDefault(selectorFunc);
    }
}
