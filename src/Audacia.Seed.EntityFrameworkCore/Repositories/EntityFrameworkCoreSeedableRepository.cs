using System.Linq.Expressions;
using Audacia.Core.Extensions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Models;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Models;
using Audacia.Seed.Properties;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Repositories;

/// <summary>
/// A seedable repository for an Entity Framework Core database context.
/// </summary>
public class EntityFrameworkCoreSeedableRepository : ISeedableRepository
{
    private readonly IList<Action> _afterSaveJobs = [];

    private readonly DbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityFrameworkCoreSeedableRepository"/> class.
    /// </summary>
    /// <param name="context">The Entity Framework context to use as a data store.</param>
    public EntityFrameworkCoreSeedableRepository(DbContext context)
    {
        _context = context;
        _afterSaveJobs =
        [
            // This removes all entity information that is currently in memory and would therefore be eagerly loaded by EF Core.
            () => _context.ChangeTracker.Clear()
        ];
    }

    /// <inheritdoc />
    public IQueryable<TEntity> DbSet<TEntity>() where TEntity : class
    {
        return _context.Set<TEntity>();
    }

    /// <inheritdoc cref="ISeedableRepository.PrepareToSet{TEntity}"/>
    public void PrepareToSet<TEntity>(TEntity? value)
    {
        if (!EqualityComparer<TEntity?>.Default.Equals(value, default) &&
            _context.Model.FindEntityType(typeof(TEntity)) != null)
        {
            _context.Entry(value!).Reload();
        }
    }

    public void SetPrimaryKey<TEntity, TKey>(TEntity entity, TKey primaryKeyValue)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(primaryKeyValue);

        var modelInformation = GetEntityModelInformation<TEntity>();

        var primaryKey = modelInformation.PrimaryKey ?? [];
        if (primaryKey.Count == 0)
        {
            throw new DataSeedingException(
                $"Cannot explicitly set the primary key for {nameof(TEntity)} as it is not configured");
        }

        if (primaryKey.Count > 1)
        {
            throw new DataSeedingException(
                $"Explicitly setting the primary key for composite keys is not supported. Entity: ${nameof(TEntity)}. Composite keys: ${string.Join(", ", primaryKey.Select(p => p.Name) ?? Array.Empty<string>())}");
        }

        var property = primaryKey.First();
        try
        {
            property.SetValue(entity, primaryKeyValue, null);
        }
        catch (ArgumentException argumentException)
        {
            throw new DataSeedingException(
                $"Unable to set the primary key. Primary key type: {property.PropertyType}. Provided type: {primaryKeyValue.GetType()}. See the inner exeception for more information",
                argumentException);
        }
    }

    /// <inheritdoc cref="ISeedableRepository.FindLocal{TEntity}"/>
    public virtual TEntity? FindLocal<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _context.Set<TEntity>().Local.FirstOrDefault(predicate.Compile());
    }

    /// <inheritdoc cref="ISeedableRepository.GetEntityModelInformation{TEntity}"/>
    public IEntityModelInformation GetEntityModelInformation<TEntity>() where TEntity : class
    {
        var entityType = _context.Model.FindEntityType(typeof(TEntity)) ??
                         throw new InvalidOperationException(
                             $"Entity type {typeof(TEntity).Name} not found in the model.");
        // This is the lowest-level base type
        var baseType = entityType.GetAllBaseTypes().FirstOrDefault(bt => bt.BaseType == null) ?? entityType;
        var foreignKeys = entityType.GetProperties().Where(p => p.IsForeignKey());

        var requiredNavigations = entityType.GetNavigations()
            .Where(n =>
            {
                var typeTheNavigationBelongsTo = n.ForeignKey.DeclaringEntityType;
                var navigationBaseType = typeTheNavigationBelongsTo.GetAllBaseTypes()
                    .FirstOrDefault(bt => bt.BaseType == null) ?? typeTheNavigationBelongsTo;
                return n.ForeignKey.IsRequired && navigationBaseType == baseType;
            })
            .Where(n => n.PropertyInfo != null)
            .Select(n =>
            {
                var matchedForeignKey =
                    foreignKeys.FirstOrDefault(fk => n.ForeignKey.Properties[0] == fk)?.PropertyInfo;
                return new NavigationPropertyConfiguration(n.PropertyInfo!, matchedForeignKey);
            })
            .ToList();

        var primaryKey = entityType.FindPrimaryKey()?.Properties
            .Where(n => n.PropertyInfo != null)
            .Select(n => n.PropertyInfo!).ToList();
        return new EntityFrameworkCoreModelInformation
        {
            EntityType = entityType.ClrType,
            RequiredNavigationProperties = requiredNavigations,
            PrimaryKey = primaryKey
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

    public T PerformSeeding<T>(EntitySeed<T> seed)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(seed);

        var entity = Seed(seed);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return entity;
    }

    public (T1 T1, T2 T2) PerformSeeding<T1, T2>(EntitySeed<T1> seed1, EntitySeed<T2> seed2)
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2);
    }

    public (T1 T1, T2 T2, T3 T3) PerformSeeding<T1, T2, T3>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2, entity3);
    }

    public (T1 T1, T2 T2, T3 T3, T4 T4) PerformSeeding<T1, T2, T3, T4>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3,
        EntitySeed<T4> seed4)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);
        var entity4 = Seed(seed4);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2, entity3, entity4);
    }

    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5) PerformSeeding<T1, T2, T3, T4, T5>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3,
        EntitySeed<T4> seed4, EntitySeed<T5> seed5)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);
        var entity4 = Seed(seed4);
        var entity5 = Seed(seed5);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2, entity3, entity4, entity5);
    }

    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6) PerformSeeding<T1, T2, T3, T4, T5, T6>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3,
        EntitySeed<T4> seed4, EntitySeed<T5> seed5, EntitySeed<T6> seed6)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed6);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);
        var entity4 = Seed(seed4);
        var entity5 = Seed(seed5);
        var entity6 = Seed(seed6);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2, entity3, entity4, entity5, entity6);
    }

    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7) PerformSeeding<T1, T2, T3, T4, T5, T6, T7>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3,
        EntitySeed<T4> seed4, EntitySeed<T5> seed5, EntitySeed<T6> seed6, EntitySeed<T7> seed7)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);
        var entity4 = Seed(seed4);
        var entity5 = Seed(seed5);
        var entity6 = Seed(seed6);
        var entity7 = Seed(seed7);

        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7);
    }

    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7, T8) PerformSeeding<T1, T2, T3, T4, T5, T6, T7, T8>(EntitySeed<T1> seed1, EntitySeed<T2> seed2, EntitySeed<T3> seed3,
        EntitySeed<T4> seed4, EntitySeed<T5> seed5, EntitySeed<T6> seed6, EntitySeed<T7> seed7, EntitySeed<T8> seed8)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);
        ArgumentNullException.ThrowIfNull(seed8);

        var entity1 = Seed(seed1);
        var entity2 = Seed(seed2);
        var entity3 = Seed(seed3);
        var entity4 = Seed(seed4);
        var entity5 = Seed(seed5);
        var entity6 = Seed(seed6);
        var entity7 = Seed(seed7);
        var entity8 = Seed(seed8);

        Save();

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7, entity8);
    }

    public void Save()
    {
        _context.SaveChanges();

        foreach (var afterSaveJob in _afterSaveJobs)
        {
            afterSaveJob();
        }
    }

    public T Seed<T>(EntitySeed<T> seed) where T : class
    {
        seed.PrepareToSeed(this);

        var entity = seed.Build();

        if (seed.Customisations.Any(c => c.GetType().GetGenericTypeDefinition() == typeof(SeedPrimaryKeyConfiguration<,>)))
        {
            EnableIdentityInsert<T>();
            _afterSaveJobs.Add(DisableIdentityInsert<T>);
        }

        Add(entity);
        // Reload the entity so that the state is `Unchanged` rather than `Detached` and we can subsequently use this entity for setting up other relationships.
        _afterSaveJobs.Add(() => _context.Entry(entity).Reload());

        return entity;
    }

    public IEnumerable<T> SeedMany<T>(int amountToCreate, EntitySeed<T> seed) where T : class
    {
        seed.PrepareToSeed(this);

        var entities = seed.BuildMany(amountToCreate).ToList();

        if (seed.Customisations.Any(c => c.GetType().GetGenericTypeDefinition() == typeof(SeedPrimaryKeyConfiguration<,>)))
        {
            EnableIdentityInsert<T>();
            _afterSaveJobs.Add(DisableIdentityInsert<T>);
        }

        foreach (var entity in entities)
        {
            seed.Repository!.Add(entity);

            // Reload the entity so that the state is `Unchanged` rather than `Detached` and we can subsequently use this entity for setting up other relationships.
            _afterSaveJobs.Add(() => _context.Entry(entity).Reload());
        }

        return entities;
    }

    private void EnableIdentityInsert<T>() where T : class
    {
        var entityType = _context.Model.FindEntityType(typeof(T))!;

        const string sqlServerProviderName = "SqlServer";
        if (_context.Database.ProviderName?.Contains(sqlServerProviderName, StringComparison.Ordinal) == true)
        {
            var query = $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} ON";
            _context.Database.ExecuteSqlRaw(query);
        }
    }

    private void DisableIdentityInsert<T>() where T : class
    {
        var entityType = _context.Model.FindEntityType(typeof(T))!;

        const string sqlServerProviderName = "SqlServer";
        if (_context.Database.ProviderName?.Contains(sqlServerProviderName, StringComparison.Ordinal) == true)
        {
            var query = $"SET IDENTITY_INSERT {entityType.GetSchema()}.{entityType.GetTableName()} OFF";
            _context.Database.ExecuteSqlRaw(query);
        }
    }
}