using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

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
    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

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
    public PrerequisiteMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        if (prerequisite.PropertyInfo == Getter.GetPropertyInfo())
        {
            return PrerequisiteMatch.Full;
        }

        return PrerequisiteMatch.None;
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

    /// <summary>
    /// Custom implementation so that we can ensure we don't apply duplicate customisations.
    /// </summary>
    /// <param name="obj">The other object to compare to.</param>
    /// <returns>Whether this object equals the <paramref name="obj"/>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is SeedNullPropertyConfiguration<TEntity, TProperty> other)
        {
            // Protect against doing the same WithDifferent twice
            return Getter.GetPropertyInfo() == other.Getter.GetPropertyInfo();
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
    }
}