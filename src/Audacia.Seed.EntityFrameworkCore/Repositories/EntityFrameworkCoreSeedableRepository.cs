using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Models;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Repositories;

/// <summary>
/// A seedable repository for an Entity Framework Core database context.
/// </summary>
public class EntityFrameworkCoreSeedableRepository : ISeedableRepository
{
    private readonly DbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkCoreSeedableRepository"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework context to use as a data store.</param>
    public EntityFrameworkCoreSeedableRepository(DbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IQueryable<TEntity> DbSet<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>();
    }

    /// <inheritdoc cref="ISeedableRepository.FindLocal{TEntity}"/>
    public virtual TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        return _context.Set<TEntity>().Local.FirstOrDefault(predicate.Compile());
    }

    /// <inheritdoc cref="ISeedableRepository.GetEntityModelInformation{TEntity}"/>
    public EntityModelInformation GetEntityModelInformation<TEntity>() where TEntity : class
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity)) ?? throw new InvalidOperationException($"Entity type {typeof(TEntity).Name} not found in the model.");

        var foreignKeys = entityType.GetForeignKeys().Where(fk => fk.IsRequired);

        var requiredNavigations = entityType.GetNavigations()
            .Where(n => n.ForeignKey.IsRequired && n.ForeignKey.DeclaringEntityType == entityType)
            .Where(n => n.PropertyInfo != null)
            .Select(n =>
            {
                var matchedForeignKey = foreignKeys.FirstOrDefault(fk => n.ForeignKey == fk)?.Properties
                    .ToList()
                    .FirstOrDefault()
                    ?.PropertyInfo;
                return new NavigationPropertyConfiguration(n.PropertyInfo!, matchedForeignKey);
            })
            .ToList();

        return new EntityModelInformation
        {
            EntityType = entityType.ClrType,
            RequiredNavigationProperties = requiredNavigations,
            PrimaryKey = entityType.FindPrimaryKey()?.Properties
                .Where(n => n.PropertyInfo != null)
                .Select(n => n.PropertyInfo!).ToList()
        };
    }

    /// <inheritdoc />
    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        // If not already in the context
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _context.Set<TEntity>().Add(entity);
        }
    }
}