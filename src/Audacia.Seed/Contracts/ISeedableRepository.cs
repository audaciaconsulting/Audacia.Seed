using System.Linq.Expressions;

namespace Audacia.Seed.Contracts;

/// <summary>
/// Interface meaning we can seed data into this for unit tests.
/// This lives in production code so that it is intentionally EF6 / EF Core agnostic.
/// </summary>
public interface ISeedableRepository
{
    /// <summary>
    /// Get an ultra read data repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity get the data set for.</typeparam>
    /// <returns>A queryable of the entire DbSet.</returns>
    IQueryable<TEntity> DbSet<TEntity>() where TEntity : class;

    /// <summary>
    /// Adds the provided entity to the underlying DbSet.
    /// </summary>
    /// <param name="entity">The entity to add to the repository.</param>
    /// <typeparam name="TEntity">The type of entity to be added.</typeparam>
    void Add<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Finds the first entity in memory that matches the provided predicate. This entity may or may not yet  be persisted to the database.
    /// </summary>
    /// <param name="predicate">A predicate to filter the DbSet by.</param>
    /// <typeparam name="TEntity">The type of entity to find.</typeparam>
    /// <returns>The first entity that matches the predicate.</returns>
    TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;
}