using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Customisation;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Seed a navigation property (via a <see cref="IEntitySeed"/>) on a provided entity with a single seed.
/// </summary>
/// <typeparam name="TEntity">The type with the property to populate.</typeparam>
/// <typeparam name="TNavigation">The type of the destination property.</typeparam>
public class SeedNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    IEntitySeed<TNavigation> seedConfiguration)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc/>
    public int Order => 50;

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body.Type != typeof(TNavigation))
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
    internal Expression<Func<TEntity, TNavigation?>> Getter { get; } = getter;

    /// <summary>
    /// Gets the seed configurations to use to populate the getter with.
    /// </summary>
    private IEntitySeed<TNavigation> SeedConfiguration { get; } = seedConfiguration;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);
        SeedConfiguration.Repository ??= repository;

        var value = (previous != null
                     && SeedConfiguration.Options.InsertionBehavior == SeedingInsertionBehaviour.TryFindNew
                        ? Getter.Compile().Invoke(previous)
                        : null)
                    ?? GetValueToSet(repository, index, previous, SeedConfiguration);

        var obj = Getter.GetPropertyOwner(entity);

        var property = Getter.GetPropertyInfo();
        property.SetValue(obj, value, null);
    }

    private static TNavigation GetValueToSet(ISeedableRepository repository, int index, TEntity? previous,
        IEntitySeed<TNavigation> navigationSeed)
    {
        var navigationSeedAsEntitySeed = navigationSeed as EntitySeed<TNavigation>;
        TNavigation? value = null;
        if ((navigationSeed.Options.InsertionBehavior == SeedingInsertionBehaviour.TryFindExisting ||
             navigationSeed.Options.InsertionBehavior != SeedingInsertionBehaviour.AddNew &&
             !navigationSeed.HasCustomisations)
            && navigationSeedAsEntitySeed != null)
        {
            value = repository.FindLocal(navigationSeedAsEntitySeed.ToPredicate(index));
        }

        if (value == null)
        {
            value = navigationSeedAsEntitySeed != null
                ? navigationSeedAsEntitySeed.GetOrCreateEntity(index, null)
                : navigationSeed.Build();
            repository.Add(value);
        }

        return value;
    }

    /// <inheritdoc/>
    public Expression<Func<TEntity, bool>> ToPredicate()
    {
        var nullCheck = Expression.NotEqual(Getter.Body, Expression.Constant(null, typeof(TNavigation)));
        var lambda = Expression.Lambda(nullCheck, Getter.Parameters);
        return (Expression<Func<TEntity, bool>>)lambda;
    }

    /// <inheritdoc/>
    public PrerequisiteMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return Getter.MatchToPrerequisite(prerequisite);
    }

    /// <summary>
    /// Custom implementation so that we can ensure we don't apply duplicate customisations.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is SeedNavigationPropertyConfiguration<TEntity, TNavigation> navigationCustomisation)
        {
            return Getter.GetPropertyInfo() == navigationCustomisation.Getter.GetPropertyInfo();
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

    /// <inheritdoc />
    public void Merge(ISeedCustomisation<TEntity> other)
    {
        if (SeedConfiguration is EntitySeed<TEntity> entitySeed
            && other is SeedNavigationPropertyConfiguration<TEntity, TNavigation>
            {
                SeedConfiguration: EntitySeed<TEntity> otherSeed
            })
        {
            SeedConfiguration.Options.Merge(otherSeed.Options);
            foreach (var customisation in otherSeed.Customisations)
            {
                entitySeed.AddCustomisation(customisation);
            }
        }
    }
}