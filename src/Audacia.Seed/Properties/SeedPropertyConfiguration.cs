using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;

namespace Audacia.Seed.Properties;

/// <summary>
/// Set a property on the provided entity to a specific value.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to set the property on.</typeparam>
/// <typeparam name="TProperty">The type of the property being set.</typeparam>
public class SeedPropertyConfiguration<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> getter, TProperty value)
    : ISeedCustomisation<TEntity>
    where TEntity : class
{
    /// <inheritdoc/>
    public int Order => int.MaxValue;

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

    /// <summary>
    /// Gets a lambda to the property to populate.
    /// </summary>
    private Expression<Func<TEntity, TProperty>> Getter { get; } = getter;

    /// <summary>
    /// Gets the value to set on the property, for all entities.
    /// </summary>
    private TProperty Value { get; } = value;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            repository.PrepareToSet(Value);
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, Value, null);
        }
    }

    /// <inheritdoc/>
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        // If this property is a foreign key, swap it out for the navigation property so we can overwrite prerequisites.
        var getter = Getter.ToNavigationProperty();

        return prerequisite.PropertyInfo == getter.GetPropertyInfo();
    }

    /// <inheritdoc/>
    public Expression<Func<TEntity, bool>> ToPredicate()
    {
        var comparison = Expression.Equal(Getter.Body, Expression.Constant(Value, typeof(TProperty)));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, Getter.Parameters);
        return lambda;
    }
}