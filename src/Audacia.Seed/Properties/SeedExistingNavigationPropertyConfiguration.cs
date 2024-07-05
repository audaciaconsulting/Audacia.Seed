using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Audacia.Seed.Contracts;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;

namespace Audacia.Seed.Properties;

/// <summary>
/// Populate a navigation property with an existing entity.
/// </summary>
/// <typeparam name="TEntity">The type the navigation property belongs to.</typeparam>
/// <typeparam name="TNavigation">The type of the property we're setting.</typeparam>
public class SeedExistingNavigationPropertyConfiguration<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation?>> getter,
    Expression<Func<TNavigation, bool>>? predicate)
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
    /// Gets an optional predicate to filter out candidates for populating this property.
    /// </summary>
    private Expression<Func<TNavigation, bool>>? Predicate { get; } = predicate;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var value = GetValueToSet(repository);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            property.SetValue(obj, value, null);
        }
    }

    private TNavigation GetValueToSet(ISeedableRepository repository)
    {
        Expression<Func<TNavigation, bool>> defaultPredicate = _ => true;
        var value = repository.FindLocal(Predicate ?? defaultPredicate) ?? TryFindFromDb(repository);

        if (value == null)
        {
            var sb = new StringBuilder($"Got a null value for the property of type {typeof(TNavigation).Name}");

            if (Predicate != null)
            {
                sb.Append(" which matches the specified predicate ");
            }

            sb.Append(" from the type ").Append(typeof(TEntity).Name);

            throw new DataSeedingException(sb.ToString());
        }

        return value;
    }

    private TNavigation? TryFindFromDb(ISeedableRepository repository)
    {
        TNavigation? value;
        var query = repository.DbSet<TNavigation>();
        if (Predicate != null)
        {
            query = query.Where(Predicate);
        }

        value = query.FirstOrDefault();
        return value;
    }

    /// <inheritdoc/>
    public Expression<Func<TEntity, bool>> ToPredicate()
    {
        var nullCheck = Expression.NotEqual(Getter.Body, Expression.Constant(null, typeof(TNavigation)));
        var lambda = Expression.Lambda(nullCheck, Getter.Parameters);
        return (Expression<Func<TEntity, bool>>)lambda;
    }

    /// <inheritdoc />
    public bool EqualsPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        return prerequisite.PropertyInfo == Getter.GetPropertyInfo();
    }

    /// <inheritdoc />
    public void Merge(ISeedCustomisation<TEntity> other)
    {
    }
}