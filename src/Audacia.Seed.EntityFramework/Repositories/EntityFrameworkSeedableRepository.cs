using System.Data.Entity;
using System.Linq.Expressions;
using Audacia.Seed.Contracts;

namespace Audacia.Seed.EntityFramework.Repositories;

/// <summary>
/// A seedable repository for a Entity Framework database context.
/// </summary>
public class EntityFrameworkSeedableRepository : ISeedableRepository
{
    private readonly DbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkSeedableRepository"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework context to use as a data store.</param>
    public EntityFrameworkSeedableRepository(DbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public IQueryable<TEntity> DbSet<TEntity>() where TEntity : class
    {
        return _context.Set(typeof(TEntity)).Cast<TEntity>();
    }

    /// <inheritdoc cref="ISeedableRepository.FindLocal{TEntity}"/>
    public virtual TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _context.Set(typeof(TEntity)).Local.Cast<TEntity>().FirstOrDefault(predicate.Compile());
    }

    /// <inheritdoc />
    public void Add<TEntity>(TEntity entity) where TEntity : class
    {
        // If not already in the context
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            // Need to use typeof to avoid `TEntity` being a proxy type.
            _context.Set(typeof(TEntity)).Add(entity);
        }
    }
}