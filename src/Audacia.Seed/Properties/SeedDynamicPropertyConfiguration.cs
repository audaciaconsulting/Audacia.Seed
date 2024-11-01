using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Contracts;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Populate a property dynamically based on a provided index and what entity was created previously.
/// </summary>
/// <typeparam name="TEntity">The type with the property to populate.</typeparam>
/// <typeparam name="TProperty">The type of the destination property.</typeparam>
internal class SeedDynamicPropertyConfiguration<TEntity, TProperty>(
    Expression<Func<TEntity, TProperty>> getter,
    Func<int, TEntity?, TProperty> valueSetter)
    : ISeedCustomisation<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <inheritdoc />
    public IEntitySeed? Seed => null;

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
    /// Gets a delegate used to return a <typeparamref name="TProperty"/> to be used to populate the property.
    /// </summary>
    private Func<int, TEntity?, TProperty> ValueSetter { get; } = valueSetter;

    /// <summary>
    /// Gets or sets the amount of values we have provided to set. Used for validation.
    /// </summary>
    internal int AmountOfValuesToSet { get; set; } = 1;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);

        var obj = Getter.GetPropertyOwner(entity);

        if (Getter.Body is MemberExpression memberSelectorExpression)
        {
            var property = (PropertyInfo)memberSelectorExpression.Member;
            try
            {
                var value = ValueSetter(index, previous);
                repository.PrepareToSet(value);
                property.SetValue(obj, value, null);
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new DataSeedingException($"Unable to set the property {Getter.GetPropertyInfo()} on {typeof(TEntity).Name} dynamically. Ensure you've passed in enough values to match the number of entities being created.", exception);
            }
        }
    }

    /// <inheritdoc/>
    public PrerequisiteMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        // If this property is a foreign key, swap it out for the navigation property so we can overwrite prerequisites.
        var getter = Getter.ToNavigationProperty();

        return getter.MatchToPrerequisite(prerequisite);
    }

    /// <inheritdoc/>
    public void Validate(EntitySeed<TEntity> entitySeed)
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        if (entitySeed.Options.AmountToCreate != AmountOfValuesToSet && AmountOfValuesToSet != 1)
        {
            throw new DataSeedingException($"We are building {entitySeed.Options.AmountToCreate} entities of type {typeof(TEntity).Name}, but {AmountOfValuesToSet} were provided.");
        }
    }

    /// <inheritdoc />
    public void Merge(ISeedCustomisation<TEntity> other)
    {
    }
}