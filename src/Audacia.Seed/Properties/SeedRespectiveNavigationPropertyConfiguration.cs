using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Constants;
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
internal class SeedRespectiveNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    List<IEntitySeed<TNavigation>> seedConfigurations)
    : ISeedCustomisation<TEntity>, INavigationSeedConfiguration
    where TEntity : class
    where TNavigation : class
{
    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <inheritdoc />
    public IEntitySeed? Seed => SeedConfigurations.FirstOrDefault();

    /// <inheritdoc />
    public IReadOnlyList<IEntitySeed> NavigationSeeds => SeedConfigurations;

    /// <inheritdoc />
    public int Order => _useDifferentSeeds ? 50 : SeedingConstants.DefaultCustomisationOrder;

    private bool _useDifferentSeeds;

    /// <inheritdoc />
    public void UseDifferentSeeds()
    {
        _useDifferentSeeds = true;
        foreach (var seed in SeedConfigurations)
        {
            seed.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
        }
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

        var value = GetValueToSet(repository, index, seedConfiguration);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, value, null);
        }
    }

    private TNavigation GetValueToSet(ISeedableRepository repository, int index, IEntitySeed<TNavigation> seedConfiguration)
    {
        var predicate = seedConfiguration is EntitySeed<TNavigation> seedConfigurationAsEntitySeed
            ? seedConfigurationAsEntitySeed.ToPredicate(index)
            : _ => true;

        var value = _useDifferentSeeds ? null : repository.FindLocal(predicate);

        if (index > 0
            && !ReferenceEquals(SeedConfigurations[index], SeedConfigurations[index - 1])
            // Force a new creation if we've explicitly provided an extra seed, even if it's identical
            && predicate.ToString() == ((Expression<Func<TNavigation, bool>>)(_ => true)).ToString())
        {
            // Force a new creation
            value = null;
        }

        if (value == null)
        {
            seedConfiguration.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
            value = seedConfiguration is IIndexedEntitySeed indexedSeed
                ? (TNavigation)indexedSeed.GetOrCreateEntity(index)
                : seedConfiguration.Build();
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