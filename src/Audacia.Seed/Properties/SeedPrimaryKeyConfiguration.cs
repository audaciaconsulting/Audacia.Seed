using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Exceptions;

namespace Audacia.Seed.Properties;

/// <summary>
/// Explicity set the primary key on <typeparamref name="TEntity"/>.
/// </summary>
/// <param name="primaryKeyValue">The primary key value to set.</param>
/// <typeparam name="TEntity">The type of the entity we are setting the primary key on.</typeparam>
public class SeedPrimaryKeyConfiguration<TEntity, TKey>(TKey[] primaryKeyValue) : ISeedCustomisation<TEntity> where TEntity : class
{
    private readonly TKey[] _primaryKeyValue = primaryKeyValue;

    /// <inheritdoc />
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        ArgumentNullException.ThrowIfNull(repository);

        try
        {
            repository.SetPrimaryKey(entity, _primaryKeyValue[index]);
        }
        catch (IndexOutOfRangeException exception)
        {
            throw new DataSeedingException($"Unable to set the primary key on {typeof(TEntity).Name} dynamically. Ensure you've passed in enough values to match the number of entities being created.", exception);
        }
    }

    /// <inheritdoc />
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

    /// <inheritdoc/>
    public void Validate(EntitySeed<TEntity> entitySeed)
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        if (entitySeed.Options.AmountToCreate != _primaryKeyValue.Length && _primaryKeyValue.Length != 1)
        {
            throw new DataSeedingException($"We are building {entitySeed.Options.AmountToCreate} entities of type {typeof(TEntity).Name}, but {_primaryKeyValue.Length} were provided.");
        }
    }
}