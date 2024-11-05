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
internal class SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    EntitySeed<TNavigation> seedConfiguration)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <inheritdoc />
    public IEntitySeed Seed => SeedConfiguration;

    /// <inheritdoc/>
    public int Order => 50;

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

        return SeedConfiguration;
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
            value = SeedConfiguration.GetOrCreateEntity(index, null);
            repository.Add(value);
        }

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
        ArgumentNullException.ThrowIfNull(entitySeed);

        // Protect against using `WithDifferent` when you're only creating one entity.
        // As `TryFindNew` is the default, this is what the seed options would look like in this scenario.
        if (entitySeed.Options is { AmountToCreate: 1, InsertionBehavior: SeedingInsertionBehaviour.TryFindNew })
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