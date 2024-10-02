using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Customisation;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Seed a navigation property (via a <see cref="IEntitySeed"/>) on a provided entity with many seeds.
/// Similar to <see cref="SeedDifferentNavigationPropertyConfiguration{TEntity,TNavigation}"/>, but some children might share the same parent.
/// </summary>
/// <typeparam name="TEntity">The type with the property to populate.</typeparam>
/// <typeparam name="TNavigation">The type of the destination property.</typeparam>
public class SeedRespectiveNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    List<IEntitySeed<TNavigation>> seedConfigurations)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, TNavigation?>> Getter { get; } = getter;

    /// <summary>
    /// Gets a list of seed configurations to use, in order in which they will be used.
    /// </summary>
    private List<IEntitySeed<TNavigation>> SeedConfigurations { get; } = seedConfigurations;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var seedConfiguration = SeedConfigurations[index];
        seedConfiguration.Repository ??= repository;

        var value = GetValueToSet(repository, index, previous, seedConfiguration);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, value, null);
        }
    }

    private TNavigation GetValueToSet(ISeedableRepository repository, int index, TEntity? previous,
        IEntitySeed<TNavigation> seedConfiguration)
    {
        Expression<Func<TNavigation, bool>> defaultPredicate = _ => true;
        var predicate = seedConfiguration is EntitySeed<TNavigation> seedConfigurationAsEntitySeed
            ? seedConfigurationAsEntitySeed.ToPredicate(index)
            : defaultPredicate;
        var value = repository.FindLocal(predicate);
        if (index > 0
            && !ReferenceEquals(SeedConfigurations[index], SeedConfigurations[index - 1])
            // Force a new creation if we've explicitly provided an extra seed, even if it's identical
            && predicate.ToString() == defaultPredicate.ToString())
        {
            // Force a new creation
            value = null;
        }

        if (value == null)
        {
            seedConfiguration.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
            value = seedConfiguration.Build();
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

        if (entitySeed.Options.AmountToCreate != SeedConfigurations.Count)
        {
            throw new DataSeedingException($"We are building {entitySeed.Options.AmountToCreate} entities of type {typeof(TEntity).Name}, but {SeedConfigurations.Count} were provided.");
        }
    }

    /// <inheritdoc />
    public void Merge(ISeedCustomisation<TEntity> other)
    {
    }
}