using System.Linq.Expressions;
using Audacia.Seed.Models;

namespace Audacia.Seed.Contracts;

/// <summary>
/// Interface meaning we can seed data into this for unit tests.
/// This lives in production code, so it is intentionally EF6 / EF Core agnostic.
/// </summary>
public interface ISeedableRepository
{
    /// <summary>
    /// Get a read-only DbSet from the repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to get the DbSet for.</typeparam>
    /// <returns>A queryable of the entire DbSet.</returns>
    IQueryable<TEntity> DbSet<TEntity>() where TEntity : class;

    /// <summary>
    /// Adds the provided entity to the underlying DbSet.
    /// </summary>
    /// <param name="entity">The entity to add to the repository.</param>
    /// <typeparam name="TEntity">The type of entity to be added.</typeparam>
    void Add<TEntity>(TEntity entity) where TEntity : class;

    /// <summary>
    /// Finds the first entity in memory that matches the provided predicate. This entity may or may not yet be persisted to the database.
    /// </summary>
    /// <param name="predicate">A predicate to filter the DbSet by.</param>
    /// <typeparam name="TEntity">The type of entity to find.</typeparam>
    /// <returns>The first entity that matches the predicate.</returns>
    TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

    /// <summary>
    /// Gets the entity model information for the provided entity type.
    /// </summary>
    /// <typeparam name="TEntity">The type to get the model information from.</typeparam>
    /// <returns>Entity model information for <typeparamref name="TEntity"/>.</returns>
    IEntityModelInformation GetEntityModelInformation<TEntity>() where TEntity : class;

    /// <summary>
    /// Set the primary key of <paramref name="entity"/> to <paramref name="primaryKeyValue"/>.
    /// </summary>
    /// <param name="entity">The entity to set the primary key on.</param>
    /// <param name="primaryKeyValue">The value of the primary key.</param>
    /// <typeparam name="TEntity">The type of entity we're setting the primary key on.</typeparam>
    /// <typeparam name="TKey">The type of the primary key we're setting.</typeparam>
    void SetPrimaryKey<TEntity, TKey>(TEntity entity, TKey primaryKeyValue) where TEntity : class;

    /// <summary>
    /// Prepare the provided <paramref name="value"/> for setting if it exists in the repository.
    /// </summary>
    /// <param name="value">The value to perpare for setting.</param>
    /// <typeparam name="TEntity">The type of entity we're looking for.</typeparam>
    void PrepareToSet<TEntity>(TEntity? value);
}