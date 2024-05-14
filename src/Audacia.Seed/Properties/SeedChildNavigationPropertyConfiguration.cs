using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;

namespace Audacia.Seed.Properties;

/// <summary>
/// Seed a child navigation property (via a <see cref="IEntitySeed"/>) on a provided entity.
/// </summary>
/// <typeparam name="TEntity">The type with the children to populate.</typeparam>
/// <typeparam name="TChildNavigation">The type of the destination property child class.</typeparam>
/// <typeparam name="TSeed">An <see cref="IEntitySeed"/> for the child navigation property.</typeparam>
public class
    SeedChildNavigationPropertyConfiguration<TEntity, TChildNavigation, TSeed>(
        Expression<Func<TEntity, IEnumerable<TChildNavigation>>> getter,
        TSeed seedConfiguration,
        int amountOfChildren)
    : ISeedCustomisation<TEntity>
    where TEntity : class
    where TChildNavigation : class
    where TSeed : EntitySeed<TChildNavigation>
{
    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, IEnumerable<TChildNavigation>>> Getter { get; } = getter;

    /// <summary>
    /// Gets a list of seed configurations to use, in order in which they will be used.
    /// </summary>
    private TSeed SeedConfiguration { get; } = seedConfiguration;

    /// <summary>
    /// Gets how many children to create.
    /// </summary>
    private int AmountOfChildren { get; } = amountOfChildren;

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

    private ICollection<TChildNavigation> GetValueToSet(
        ISeedableRepository repository,
        int index,
        TEntity? previous)
    {
        var validEntities = SeedConfiguration.BuildMany(AmountOfChildren).AsQueryable();

        List<TChildNavigation> value = validEntities.ToList();

        return value;
    }

    /// <inheritdoc/>
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return prerequisite.PropertyInfo == Getter.GetPropertyInfo();
    }
}