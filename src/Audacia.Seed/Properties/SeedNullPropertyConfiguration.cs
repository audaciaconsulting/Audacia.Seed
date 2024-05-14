using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;

namespace Audacia.Seed.Properties;

/// <summary>
/// Set a property belonging to the entity as null.
/// </summary>
/// <typeparam name="TEntity">The type of the entity to set the property as null on.</typeparam>
/// <typeparam name="TProperty">The type of the property to set as null. Must be nullable.</typeparam>
public class SeedNullPropertyConfiguration<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> getter)
    : ISeedCustomisation<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Gets a lambda to the property to set as null.
    /// </summary>
    private Expression<Func<TEntity, TProperty>> Getter { get; } = getter;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, null, null);
        }
    }

    /// <inheritdoc/>
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return prerequisite.PropertyInfo == Getter.GetPropertyInfo();
    }

    /// <inheritdoc/>
    public Expression<Func<TEntity, bool>> ToPredicate()
    {
        var comparison = Expression.Equal(Getter.Body, Expression.Constant(default(TProperty)));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, Getter.Parameters);
        return lambda;
    }

    /// <inheritdoc/>
    public void Validate(EntitySeed<TEntity> entitySeed)
    {
    }
}