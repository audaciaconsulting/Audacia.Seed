using System.Linq.Expressions;
using System.Reflection;
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
/// <typeparam name="TSeed">An <see cref="IEntitySeed"/> for the navigation property.</typeparam>
public class SeedNavigationPropertyConfiguration<TEntity, TNavigation, TSeed>(
    Expression<Func<TEntity, TNavigation?>> getter,
    TSeed seedConfiguration)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TNavigation : class
    where TSeed : EntitySeed<TNavigation>
{
    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, TNavigation?>> Getter { get; } = getter;

    /// <summary>
    /// Gets a list of seed configurations to use, in order in which they will be used.
    /// </summary>
    private TSeed SeedConfiguration { get; } = seedConfiguration;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);
        SeedConfiguration.Repository ??= repository;

        var value = (previous != null
                     && SeedConfiguration.Options.InsertionBehavior == SeedingInsertionBehaviour.TryFindExisting
                        ? Getter.Compile().Invoke(previous)
                        : null)
                    ?? GetValueToSet(repository, index, previous, SeedConfiguration);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, value, null);
        }
    }

    private static TNavigation GetValueToSet(ISeedableRepository repository, int index, TEntity? previous,
        TSeed navigationSeed)
    {
        TNavigation? value = null;
        if (navigationSeed.Options.InsertionBehavior == SeedingInsertionBehaviour.MustFindExisting ||
            (navigationSeed.Options.InsertionBehavior != SeedingInsertionBehaviour.AddNew && !navigationSeed.HasCustomisations))
        {
            value = repository.FindLocal(navigationSeed.ToPredicate(index));
        }

        if (value == null)
        {
            value = navigationSeed.Build();
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
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return prerequisite.PropertyInfo == Getter.GetPropertyInfo();
    }
}