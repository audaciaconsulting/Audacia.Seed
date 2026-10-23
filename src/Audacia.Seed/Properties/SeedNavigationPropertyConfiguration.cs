using System.Diagnostics.CodeAnalysis;
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
internal class SeedNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    IEntitySeed<TNavigation> seedConfiguration)
    : ISeedCustomisation<TEntity>, INavigationSeedConfiguration
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc/>
    public int Order => 50;

    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <inheritdoc />
    public IEntitySeed Seed => SeedConfiguration;

    /// <inheritdoc />
    public IReadOnlyList<IEntitySeed> NavigationSeeds => [SeedConfiguration];

    private bool _useDifferentSeeds;

    /// <inheritdoc />
    public void UseDifferentSeeds()
    {
        // Don't change the seed's options, as the seed may be shared with other navigation properties.
        _useDifferentSeeds = true;
    }

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        ArgumentNullException.ThrowIfNull(getter);

        return getter.Body.Type != typeof(TNavigation) || getter.GetPropertyInfo() != Getter.GetPropertyInfo()
            ? null
            : (IEntitySeed?)(SeedConfiguration as EntitySeed<TNavigation>);
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

        var value = (UsePrevious(previous) ? Getter.Compile().Invoke(previous) : null)
            ?? GetValueToSet(repository, index, SeedConfiguration);

        var obj = Getter.GetPropertyOwner(entity);

        var property = Getter.GetPropertyInfo();
        property.SetValue(obj, value, null);
    }

    private bool UsePrevious([NotNullWhen(true)] TEntity? previous)
    {
        return previous != null
            && !_useDifferentSeeds
            && SeedConfiguration.Options.InsertionBehavior == SeedingInsertionBehaviour.TryFindNew;
    }

    /// <summary>
    /// Find or create the navigation for this index, always creating one if made different via <c>WithDifferent</c>.
    /// Uses <see cref="IIndexedEntitySeed"/> so a derived seed, e.g. <c>EntitySeed&lt;EmployeeAsset&gt;</c>, keeps the index.
    /// </summary>
    private TNavigation GetValueToSet(ISeedableRepository repository, int index,
        IEntitySeed<TNavigation> navigationSeed)
    {
        TNavigation? value = null;

        if (!_useDifferentSeeds
            && CanReuseExistingNavigation(navigationSeed)
            && navigationSeed is EntitySeed<TNavigation> navigationSeedAsEntitySeed)
        {
            value = repository.FindLocal(navigationSeedAsEntitySeed.ToPredicate(index));
        }

        if (value == null)
        {
            value = navigationSeed switch
            {
                IIndexedEntitySeed indexedSeed when _useDifferentSeeds => (TNavigation)indexedSeed.CreateEntity(index),
                IIndexedEntitySeed indexedSeed => (TNavigation)indexedSeed.GetOrCreateEntity(index),
                _ => navigationSeed.Build()
            };
            repository.Add(value);
        }

        return value;
    }

    private static bool CanReuseExistingNavigation(IEntitySeed<TNavigation> navigationSeed)
    {
        return navigationSeed.Options.InsertionBehavior == SeedingInsertionBehaviour.TryFindExisting ||
            (navigationSeed.Options.InsertionBehavior != SeedingInsertionBehaviour.AddNew &&
            !navigationSeed.HasCustomisations);
    }

    /// <inheritdoc/>
    public Expression<Func<TEntity, bool>> ToPredicate()
    {
        var nullCheck = Expression.NotEqual(Getter.Body, Expression.Constant(null, typeof(TNavigation)));
        var lambda = Expression.Lambda(nullCheck, Getter.Parameters);
        return (Expression<Func<TEntity, bool>>)lambda;
    }

    /// <inheritdoc/>
    public LambdaExpressionMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return Getter.MatchToPrerequisite(prerequisite);
    }

    /// <inheritdoc/>
    public void Validate(EntitySeed<TEntity> entitySeed)
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        if (_useDifferentSeeds)
        {
            INavigationSeedConfiguration.ValidateDifferentSeeds(entitySeed);
        }
    }

    /// <summary>
    /// Custom implementation so that we can ensure we don't apply duplicate customisations.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is SeedNavigationPropertyConfiguration<TEntity, TNavigation> navigationCustomisation
            && Getter.GetPropertyInfo() == navigationCustomisation.Getter.GetPropertyInfo();
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