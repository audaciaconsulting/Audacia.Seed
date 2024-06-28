using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Customisation;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Seed a navigation property (via a <see cref="IEntitySeed"/>) on a provided entity with a newly constructed entity.
/// </summary>
/// <typeparam name="TEntity">The type with the property to populate.</typeparam>
/// <typeparam name="TNavigation">The type of the destination property.</typeparam>
/// <typeparam name="TSeed">An <see cref="IEntitySeed"/> for the navigation property.</typeparam>
public class SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation, TSeed>(
    Expression<Func<TEntity, TNavigation?>> getter,
    TSeed seedConfiguration)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TNavigation : class
    where TSeed : EntitySeed<TNavigation>
{
    /// <inheritdoc/>
    public int Order => 0;

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Parameters[0].Type != typeof(TNavigation))
        {
            return null;
        }

        if (getter.GetPropertyInfo() != Getter.GetPropertyInfo())
        {
            return null;
        }

        var seedToReturn = SeedConfiguration as EntitySeed<TNavigation>;
        return seedToReturn;
    }

    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, TNavigation?>> Getter { get; } = getter;

    /// <summary>
    /// Gets the seed configuration to use.
    /// </summary>
    private TSeed SeedConfiguration { get; } = seedConfiguration;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);
        SeedConfiguration.Repository ??= repository;

        var value = GetValueToSet(repository, index, previous);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, value, null);
        }
    }

    private TNavigation GetValueToSet(ISeedableRepository repository, int index, TEntity? previous)
    {
        TNavigation? value = null;
        if (SeedConfiguration.Options.InsertionBehavior != SeedingInsertionBehaviour.AddNew)
        {
            value = repository.FindLocal(SeedConfiguration.ToPredicate(index));
        }

        if (value == null)
        {
            SeedConfiguration.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
            value = SeedConfiguration.Build();
            repository.Add(value);
        }

        return value;
    }

    /// <inheritdoc/>
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return prerequisite.PropertyInfo == Getter.GetPropertyInfo();
    }

    /// <inheritdoc/>
    public void Validate(EntitySeed<TEntity> entitySeed)
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        if (entitySeed.Options is { AmountToCreate: 1, InsertionBehavior: SeedingInsertionBehaviour.TryFindExisting })
        {
            throw new DataSeedingException($"`{nameof(EntitySeedExtensions.WithDifferent)} was specified, but we're only building 1 {typeof(TEntity).Name}.");
        }
    }

    /// <summary>
    /// Custom implementation so that we can ensure we don't apply duplicate customisations.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation, TSeed> other)
        {
            // Protect against doing the same WithDifferent twice
            return Getter.GetPropertyInfo() == other.Getter.GetPropertyInfo();
        }

        return false;
    }

    /// <summary>
    /// Custom implementation so that equality operations only care about the type.
    /// </summary>
    /// <returns>The hashcode unique to this.</returns>
    public override int GetHashCode()
    {
        return Getter.GetPropertyInfo().GetHashCode();
    }
}