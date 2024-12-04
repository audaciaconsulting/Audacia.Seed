using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Customisation;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;

namespace Audacia.Seed;

/// <summary>
/// Something that is required to seed an entity.
/// </summary>
/// <typeparam name="TEntity">The type that this prerequisite is for.</typeparam>
/// <typeparam name="TNavigation">The type of the property that is a prerequisite e.g navigation property.</typeparam>
public class SeedPrerequisite<TEntity, TNavigation> : ISeedPrerequisite
    where TEntity : class
    where TNavigation : class
{
    /// <summary>
    /// Gets a getter for the navigation property.
    /// </summary>
    public Expression<Func<TEntity, TNavigation>> Getter { get; }

    /// <inheritdoc />
    public IEntitySeed Seed { get; }

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

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedPrerequisite{TEntity, TNavigation}"/> class.
    /// </summary>
    /// <param name="getter">A getter to the navigation property.</param>
    /// <param name="seed">A seed class for the navigation property.</param>
    public SeedPrerequisite(
        Expression<Func<TEntity, TNavigation>> getter,
        EntitySeed<TNavigation>? seed)
    {
        Getter = getter;
        Seed = seed
               ?? EntryPointAssembly.Load().FindSeed(typeof(TNavigation))
               ?? throw new DataSeedingException(
                   $"Unable to find an appropriate seed for the entity {typeof(TEntity).Name} and getter {Getter}.");
    }
}

/// <summary>
/// Something that is required to seed an entity.
/// </summary>
/// <typeparam name="TEntity">The type that this prerequisite is for.</typeparam>
/// <typeparam name="TNavigation">The type of the property that is a prerequisite e.g navigation property.</typeparam>
public class SeedChildrenPrerequisite<TEntity, TNavigation> : ISeedPrerequisite
    where TEntity : class
    where TNavigation : class
{
    /// <summary>
    /// Gets a getter for the navigation property.
    /// </summary>
    public Expression<Func<TEntity, IEnumerable<TNavigation>>> Getter { get; }

    /// <inheritdoc />
    public IEntitySeed Seed { get; }

    /// <inheritdoc />
    public Type EntityType => typeof(TNavigation);

    /// <inheritdoc />
    public PropertyInfo PropertyInfo => Getter.GetPropertyInfo();

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedPrerequisite{TEntity, TNavigation, TChildNavigation}"/> class.
    /// </summary>
    /// <param name="getter">A getter to the navigation property.</param>
    /// <param name="parentSeed">A seed class for the parent of the navigation property.</param>
    /// <param name="seed">A seed class for the navigation property.</param>
    /// <param name="numberOfChildren">The number of children seeds to place within the navigation property.</param>
    public SeedChildrenPrerequisite(
        Expression<Func<TEntity, IEnumerable<TNavigation>>> getter,
        EntitySeed<TNavigation>? seed,
        int numberOfChildren)
    {
        Getter = getter;
        Seed = seed
               ?? EntryPointAssembly.Load().FindSeed(typeof(TNavigation))
               ?? throw new DataSeedingException(
                   $"Unable to find an appropriate seed for the entity {typeof(TEntity).Name} and getter {Getter}.");
        Seed.Options.AmountToCreate = numberOfChildren;
    }

    public SeedChildrenPrerequisite(
        Expression<Func<TEntity, IEnumerable<TNavigation>>> getter,
        EntitySeed<TNavigation>? seed) : this(getter, seed, 1)
    {
    }
}