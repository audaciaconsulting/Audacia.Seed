using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;

namespace Audacia.Seed;

/// <summary>
/// Something that is required to seed an entity.
/// </summary>
/// <typeparam name="TEntity">The type that this prerequisite is for.</typeparam>
/// <typeparam name="TNavigation">The type of the property that is a prerequisite e.g navigation property.</typeparam>
public class SeedPrerequisite<TEntity, TNavigation>(
    Expression<Func<TEntity, TNavigation>> getter,
    EntitySeed<TNavigation>? seed)
    : ISeedPrerequisite
    where TEntity : class
    where TNavigation : class
{
    /// <summary>
    /// Gets a getter for the navigation property.
    /// </summary>
    public Expression<Func<TEntity, TNavigation>> Getter { get; } = getter;

    private readonly EntitySeed<TNavigation>? _seed = seed;

    /// <inheritdoc />
    public Type EntityType => typeof(TNavigation);

    /// <inheritdoc />
    public PropertyInfo PropertyInfo => Getter.GetPropertyInfo();

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedPrerequisite{TEntity, TNavigation}"/> class.
    /// </summary>
    /// <param name="getter">A getter to the navigation property.</param>
    public SeedPrerequisite(Expression<Func<TEntity, TNavigation>> getter) : this(getter, null)
    {
    }

    /// <inheritdoc />
    public IEntitySeed GetSeed()
    {
        return _seed
               ?? EntryPointAssembly.Load().FindSeed(typeof(TNavigation))
               ?? throw new DataSeedingException(
                   $"Unable to find an appropriate seed for the entity {typeof(TEntity).Name} and getter {Getter}.");
    }
}