using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;
using Audacia.Seed.Models;

namespace Audacia.Seed.InMemory;

/// <summary>
/// A seedable repository for in-memory data.
/// </summary>
internal class InMemorySeedableRepository : ISeedableRepository
{
    private readonly Dictionary<Type, IEnumerable> _dbSets = new();

    /// <inheritdoc />
    public IQueryable<TEntity> DbSet<TEntity>() where TEntity : class
    {
        var data = _dbSets.SafeGetValue(typeof(TEntity), () => new List<TEntity>());
        return data.Cast<TEntity>().AsQueryable();
    }

    /// <inheritdoc />
    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        var data = DbSet<TEntity>().ToList();
        data.Add(entity);
        this._dbSets[typeof(TEntity)] = data;
    }

    /// <inheritdoc cref="ISeedableRepository.FindLocal{TEntity}"/>
    public TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        return DbSet<TEntity>()
            .Where(predicate)
            .FirstOrDefault();
    }

    /// <inheritdoc cref="ISeedableRepository.GetEntityModelInformation{TEntity}"/>
    public IEntityModelInformation GetEntityModelInformation<TEntity>() where TEntity : class
    {
        var requiredNavigationProperties = typeof(TEntity).GetRequiredNavigationProperties()
            .Select(p => new NavigationPropertyConfiguration(p, null))
            .ToList();

        return new InMemoryModelInformation
        {
            EntityType = typeof(TEntity),
            RequiredNavigationProperties = requiredNavigationProperties!
        };
    }

    /// <inheritdoc />
    public void SetPrimaryKey<TEntity, TKey>(TEntity entity, TKey primaryKeyValue) where TEntity : class
    {
        throw new NotSupportedException("Setting the primary key for an in-memory entity is not supported");
    }

    /// <inheritdoc cref="ISeedableRepository.PrepareToSet{TEntity}"/>
    [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Required by the interface.")]
    public void PrepareToSet<TEntity>(TEntity? value)
    {
    }
}