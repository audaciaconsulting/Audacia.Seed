using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Seed a navigation property (via a <see cref="IEntitySeed"/>) on a provided entity with a newly constructed entity.
/// </summary>
/// <typeparam name="TEntity">The type with the property to populate.</typeparam>
/// <typeparam name="TNavigation">The type of the destination property.</typeparam>
internal class SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    EntitySeed<TNavigation> seedConfiguration)
    : ISeedCustomisation<TEntity>, INavigationSeedConfiguration
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <inheritdoc />
    public IEntitySeed Seed => SeedConfiguration;

    /// <inheritdoc />
    public IReadOnlyList<IEntitySeed> NavigationSeeds => [SeedConfiguration];

    /// <inheritdoc/>
    public int Order => 50;

    /// <inheritdoc />
    public void UseDifferentSeeds()
    {
        // This configuration always creates a new navigation entity.
    }

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        ArgumentNullException.ThrowIfNull(getter);

        return getter.Parameters[0].Type != typeof(TNavigation) || getter.GetPropertyInfo() != Getter.GetPropertyInfo()
            ? null
            : SeedConfiguration;
    }

    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, TNavigation?>> Getter { get; } = getter;

    /// <summary>
    /// Gets the seed configuration to use.
    /// </summary>
    private EntitySeed<TNavigation> SeedConfiguration { get; } = seedConfiguration;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);
        SeedConfiguration.Repository ??= repository;

        var value = GetValueToSet(repository, index);

        var obj = Getter.GetPropertyOwner(entity);

        // The getter may be a cast to a derived type, e.g. x => (EmployeeAsset)x.Asset, so get the property from inside any cast.
        var property = Getter.GetPropertyInfo();
        property.SetValue(obj, value, null);
    }

    private TNavigation GetValueToSet(ISeedableRepository repository, int index)
    {
        var value = (TNavigation)((IIndexedEntitySeed)SeedConfiguration).CreateEntity(index);
        repository.Add(value);
        return value;
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
        INavigationSeedConfiguration.ValidateDifferentSeeds(entitySeed);
    }

    /// <summary>
    /// Custom implementation so that we can ensure we don't apply duplicate customisations.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation> differentCustomisation)
        {
            // Protect against doing the same WithDifferent twice
            return Getter.GetPropertyInfo() == differentCustomisation.Getter.GetPropertyInfo();
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
        if (other is SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation>
            otherCustomisation)
        {
            SeedConfiguration.Options.Merge(otherCustomisation.SeedConfiguration.Options);
            foreach (var customisation in otherCustomisation.SeedConfiguration.Customisations)
            {
                SeedConfiguration.AddCustomisation(customisation);
            }
        }
    }
}