using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Repositories;

/// <summary>
/// A seedable repository for an Entity Framework Core database context.
/// </summary>
internal class EntityFrameworkCoreSeedableRepository : ISeedableRepository
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