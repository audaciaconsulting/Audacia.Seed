using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Audacia.Core.Extensions;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;
using Audacia.Seed.Options;
using Audacia.Seed.Properties;

namespace Audacia.Seed.Customisation;

/// <summary>
/// Extensions for the <see cref="EntitySeed{TEntity}"/> type.
/// </summary>
public static class EntitySeedExtensions
{
    /// <summary>
    /// Set a property on the entity to the specified <paramref name="value"/>.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="value">The value to set on the property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> With<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter,
        TProperty value)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(new SeedPropertyConfiguration<TEntity, TProperty>(getter, value));

        return entitySeed;
    }

    /// <summary>
    /// Set a property on entities with the specified <paramref name="values"/>.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="values">The values to set in order based on how many we're creating.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="ArgumentException">If no <paramref name="values"/> are provided.</exception>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> With<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter,
        params TProperty[] values)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(values);
        ArgumentNullException.ThrowIfNull(getter);

        if (values.Length == 0)
        {
            throw new ArgumentException("You must provide at least one value to set.");
        }

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        // Check the getter passed in is setting a nested property or not
        // E.g x => x.Parent.GrandParentId is nested, but x => x.ParentId is not.
        if (getter.Body is MemberExpression m
            && m.Expression?.Type != typeof(TEntity) && values.Length > 1)
        {
            // If specifying > 1, we will therefore want each parent to be different if the property belongs to a parent.
            var (left, _) = getter.SplitLastMemberAccessLayer();
            entitySeed.WithDifferentReflection(left, typeof(TEntity), left.Body.Type);
        }

        var customisation =
            new SeedDynamicPropertyConfiguration<TEntity, TProperty>(getter, (index, _) => values[index])
            {
                AmountOfValuesToSet = values.Length
            };

        entitySeed.AddCustomisation(customisation);

        return entitySeed;
    }

    /// <summary>
    /// Set a property on entities using the provided <paramref name="valueSetter"/>.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="valueSetter">A delegate to set the property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> With<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter,
        Func<int, TEntity?, TProperty> valueSetter)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(valueSetter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(new SeedDynamicPropertyConfiguration<TEntity, TProperty>(getter, valueSetter));
        return entitySeed;
    }

    /// <summary>
    /// Set a property on entities using the provided <paramref name="valueSetter"/>.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="valueSetter">A delegate to set the property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> With<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter,
        Func<int, TProperty> valueSetter)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(valueSetter);

        TProperty ValueSetterWithEntity(int index, TEntity? entity)
        {
            return valueSetter(index);
        }

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(
            new SeedDynamicPropertyConfiguration<TEntity, TProperty>(getter, ValueSetterWithEntity));

        return entitySeed;
    }

    /// <summary>
    /// Set a property on entities using the provided <paramref name="valueSetter"/>.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="valueSetter">A delegate to set the property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> With<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter,
        Func<TProperty> valueSetter)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);
        ArgumentNullException.ThrowIfNull(valueSetter);

        TProperty ValueSetterWithEntity(int index, TEntity? entity)
        {
            return valueSetter();
        }

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(
            new SeedDynamicPropertyConfiguration<TEntity, TProperty>(getter, ValueSetterWithEntity));
        return entitySeed;
    }

    /// <summary>
    /// Set a property on the entity as null.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set as null.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TProperty">The type of the property so set. This should be nullable.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> Without<TEntity, TProperty>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TProperty>> getter)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(new SeedNullPropertyConfiguration<TEntity, TProperty>(getter));
        return entitySeed;
    }

    /// <summary>
    /// Specify that the property should have different navigation properties.
    /// <br/>
    /// To be used if building > 1 entity.
    /// <br/>
    /// Usage note: if you want to control how the entities are different, use the WithNew method, explicitly providing the configurations.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    public static EntitySeed<TEntity> WithDifferent<TEntity, TNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter)
        where TEntity : class
        where TNavigation : class
    {
        var assembly = EntryPointAssembly.Load();
        var seedConfiguration = assembly.FindSeed<TNavigation>();
        return entitySeed.WithDifferent(getter, seedConfiguration);
    }

    /// <summary>
    /// Specify that the property should have different navigation properties, based on the provided <paramref name="seedConfiguration"/>.
    /// <br/>
    /// To be used if building > 1 entity.
    /// <br/>
    /// Usage note: if you want to control how the entities are different, use the <see cref="With{TEntity,TProperty}(EntitySeed{TEntity},Expression{Func{TEntity,TProperty}},TProperty)"/> method subsequently.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="seedConfiguration">The seed configuration to use for each navigation property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <typeparam name="TSeed">Seed configuration for <typeparamref name="TNavigation"/>.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    private static EntitySeed<TEntity> WithDifferent<TEntity, TNavigation, TSeed>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter,
        TSeed seedConfiguration)
        where TEntity : class
        where TNavigation : class
        where TSeed : EntitySeed<TNavigation>
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(seedConfiguration);
        ArgumentNullException.ThrowIfNull(getter);

        // The getter may be a cast to a derived type, e.g. x => (EmployeeAsset)x.Asset, so validate the property inside any cast.
        var member = getter.Body switch
        {
            MemberExpression memberExpression => memberExpression,
            UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression operand } => operand,
            _ => throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.")
        };

        seedConfiguration.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;

        // Check the getter passed in is setting a nested property or not
        // E.g x => x.Parent.GrandParent is nested, but x => x.Parent is not.
        if (member.Expression?.Type != typeof(TEntity))
        {
            // Add a WithDifferent for the next level, which will recursively do the same until we reach the end.
            WithDifferentRecursive(entitySeed, getter);
        }
        else
        {
            // Retain the existing configuration's position and all explicitly supplied seeds.
            var existing = FindExistingNavigationSeeds(entitySeed, getter);
            if (existing != null)
            {
                existing.UseDifferentSeeds();
            }
            else
            {
                entitySeed.AddCustomisation(new SeedDifferentNavigationPropertyConfiguration<TEntity, TNavigation>(
                    getter,
                    seedConfiguration));
            }
        }

        return entitySeed;
    }

    /// <summary>
    /// Ensure the provieded property is populated as a predecessor to this entity.
    /// <br />
    /// This will try to find a seed in the assembly running the tests, and fallback on a <see cref="EntitySeed{TEntity}"/> if none can be found.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    [SuppressMessage("Naming", "CA1711: Identifiers should not have incorrect suffix",
        Justification = "This is deliberately the opposite of `WithExisting`, so `New` is meaningful here.")]
    public static EntitySeed<TEntity> WithNew<TEntity, TNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter)
        where TEntity : class
        where TNavigation : class
    {
        var assembly = EntryPointAssembly.Load();
        var seedConfiguration = assembly.FindSeed<TNavigation>();
        return entitySeed.WithNew(getter, seedConfiguration);
    }

    /// <summary>
    /// Override the default behaviour to set a custom <see cref="IEntitySeed"/> as a prerequisite to building this entity.
    /// <br />
    /// This can be used to override the default prerequisite set, or to specify one that wouldn't have been set by default, for example if it were an optional relationship.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="seedConfigurations">The seed configurations to use for each navigation property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="ArgumentException">If no seed configurations are provided.</exception>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    [SuppressMessage("Naming", "CA1711: Identifiers should not have incorrect suffix",
        Justification = "This is deliberately the opposite of `WithExisting`, so `New` is meaningful here.")]
    public static EntitySeed<TEntity> WithNew<TEntity, TNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter,
        params IEntitySeed<TNavigation>[] seedConfigurations)
        where TEntity : class
        where TNavigation : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(seedConfigurations);
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        if (seedConfigurations.Length == 0)
        {
            throw new ArgumentException("You must provide at least one seed configuration.");
        }

        if (seedConfigurations.Length > 1)
        {
            entitySeed.AddCustomisation(new SeedRespectiveNavigationPropertyConfiguration<TEntity, TNavigation>(
                getter,
                [.. seedConfigurations]));
        }
        else
        {
            entitySeed.AddCustomisation(new SeedNavigationPropertyConfiguration<TEntity, TNavigation>(
                getter,
                seedConfigurations[0]));
        }

        return entitySeed;
    }

    /// <summary>
    /// Override the default behaviour to set a custom <see cref="IEntitySeed"/> on a child property as a prerequisite to building this entity.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TChildNavigation">The type of the child navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    public static EntitySeed<TEntity> WithChildren<TEntity, TChildNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, IEnumerable<TChildNavigation>>> getter)
        where TEntity : class
        where TChildNavigation : class
    {
        var seedConfiguration = EntryPointAssembly.Load().FindSeed<TChildNavigation>();
        const int amountOfChildren = 1;
        return entitySeed.WithChildren(getter, amountOfChildren, seedConfiguration);
    }

    /// <summary>
    /// Override the default behaviour to set a custom <see cref="IEntitySeed"/> on a child property as a prerequisite to building this entity.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="amountOfChildren">The number of children to create.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TChildNavigation">The type of the child navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    public static EntitySeed<TEntity> WithChildren<TEntity, TChildNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, IEnumerable<TChildNavigation>>> getter,
        int amountOfChildren)
        where TEntity : class
        where TChildNavigation : class
    {
        var seedConfiguration = EntryPointAssembly.Load().FindSeed<TChildNavigation>();
        return entitySeed.WithChildren(getter, amountOfChildren, seedConfiguration);
    }

    /// <summary>
    /// Override the default behaviour to set a custom <see cref="IEntitySeed"/> on a child property as a prerequisite to building this entity.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="amountOfChildren">The number of children to create.</param>
    /// <param name="seedConfiguration">The seed configuration to use for the child property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TChildNavigation">The type of the child navigation property to set.</typeparam>
    /// <typeparam name="TSeed">Seed configuration for <typeparamref name="TChildNavigation"/>.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> WithChildren<TEntity, TChildNavigation, TSeed>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, IEnumerable<TChildNavigation>>> getter,
        int amountOfChildren,
        TSeed seedConfiguration)
        where TEntity : class
        where TChildNavigation : class
        where TSeed : EntitySeed<TChildNavigation>
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(new SeedChildNavigationPropertyConfiguration<TEntity, TChildNavigation, TSeed>(
            getter,
            seedConfiguration,
            amountOfChildren));
        return entitySeed;
    }

    /// <summary>
    /// Specify that the navigation property specified should use an existing entry from the database.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    public static EntitySeed<TEntity> WithExisting<TEntity, TNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter)
        where TEntity : class
        where TNavigation : class
    {
        return entitySeed.WithExisting(getter, null);
    }

    /// <summary>
    /// Specify that the navigation property specified should use an existing entry from the database.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="getter">A lambda to the property to set.</param>
    /// <param name="predicate">An optional predicate to filter the existing entities to set.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TNavigation">The type of the navigation property to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    /// <exception cref="DataSeedingException">If the provided expression does not access data on a <typeparamref name="TEntity"/>.</exception>
    public static EntitySeed<TEntity> WithExisting<TEntity, TNavigation>(
        this EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter,
        Expression<Func<TNavigation, bool>>? predicate)
        where TEntity : class
        where TNavigation : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);
        ArgumentNullException.ThrowIfNull(getter);

        if (getter.Body is not MemberExpression)
        {
            throw new DataSeedingException(
                $"The provided {nameof(getter)} ({getter}) does not access a property on {typeof(TEntity).Name}.");
        }

        entitySeed.AddCustomisation(
            new SeedExistingNavigationPropertyConfiguration<TEntity, TNavigation>(getter, predicate));
        return entitySeed;
    }

    /// <summary>
    /// Explicitly set the primary key, rather than it being automatically generated on save.
    /// </summary>
    /// <param name="entitySeed">The seed class.</param>
    /// <param name="primaryKeyValues">The primary key values to set in order.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <typeparam name="TKey">The type of the primary key to set.</typeparam>
    /// <returns>This object containing this additional customisation.</returns>
    public static EntitySeed<TEntity> WithPrimaryKey<TEntity, TKey>(
        this EntitySeed<TEntity> entitySeed,
        params TKey[] primaryKeyValues)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        entitySeed.AddCustomisation(new SeedPrimaryKeyConfiguration<TEntity, TKey>(primaryKeyValues));
        return entitySeed;
    }

    /// <summary>
    /// Return a predicate that can be used to match an entity seed to an entity.
    /// </summary>
    /// <param name="seed">The seed to get the predicated for.</param>
    /// <param name="index">The index of the entity being added.</param>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <returns>A predicate that can be used for matching.</returns>
    public static Expression<Func<TEntity, bool>> ToPredicate<TEntity>(this EntitySeed<TEntity> seed, int index)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(seed);

        var predicate = seed.Predicate(index);

        return !seed.HasCustomisations
            ? predicate
            : predicate.And(seed.Customisations.Select(c => c.ToPredicate()).Aggregate((first, second) => first.And(second)));
    }

    private static void WithDifferentRecursive<TEntity, TNavigation>(
        EntitySeed<TEntity> entitySeed,
        Expression<Func<TEntity, TNavigation?>> getter)
        where TEntity : class
        where TNavigation : class
    {
        var (left, right) = getter.SplitFirstMemberAccessLayer();

        // For a x => x.Parent.GrandParent, get a EntitySeed<Parent>, with WithDifferent applied
        foreach (var parentSeed in GetOrCreateParentSeeds(entitySeed, left, 1))
        {
            parentSeed.WithDifferentReflection(right, left.Body.Type, typeof(TNavigation));
        }
    }

    private static void WithDifferentReflection(
        this IEntitySeed seed,
        LambdaExpression getter,
        Type sourceType,
        Type destinationType)
    {
        // EntitySeed<T> is invariant, even though WithNew accepts covariant IEntitySeed<T>.
        // Rebind a base-property lambda to the actual seed type before invoking WithDifferent.
        if (sourceType != seed.EntityType)
        {
            var parameter = Expression.Parameter(seed.EntityType, getter.Parameters[0].Name);
            var body = new NavigationParameterVisitor(getter.Parameters[0], parameter).Visit(getter.Body)!;
            getter = Expression.Lambda(body, parameter);
            sourceType = seed.EntityType;
        }

        var withDifferent = typeof(EntitySeedExtensions).GetMethods()
            .Single(method => method.Name == nameof(WithDifferent) && method.GetParameters().Length == 2)
            .MakeGenericMethod(sourceType, destinationType);
        object[] args = [seed, getter];
        withDifferent.Invoke(null, args);
    }

    private static IReadOnlyList<IEntitySeed> GetOrCreateParentSeeds<TEntity>(
        EntitySeed<TEntity> entitySeed,
        LambdaExpression left,
        int amountToCreate)
        where TEntity : class
    {
        var existing = FindExistingNavigationSeeds(entitySeed, left);
        if (existing != null)
        {
            return UseExistingSeedsForDifferent(existing, amountToCreate);
        }

        var seed = entitySeed.Customisations.Select(c => c.FindSeedForGetter(left)).FirstOrDefault(s => s != null)
                   ?? EntryPointAssembly.Load().FindSeed(left.Body.Type);

        seed.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
        seed.Options.AmountToCreate = amountToCreate;

        AddWithDifferentForNavigationProperty(entitySeed, left, seed);
        return [seed];
    }

    private static IReadOnlyList<IEntitySeed> UseExistingSeedsForDifferent(INavigationSeedConfiguration existing, int amountToCreate)
    {
        existing.UseDifferentSeeds();
        foreach (var navigationSeed in existing.NavigationSeeds)
        {
            navigationSeed.Options.AmountToCreate = amountToCreate;
        }

        return existing.NavigationSeeds;
    }

    /// <summary>
    /// Find an existing navigation customisation so it can be augmented in place, without
    /// moving it behind dependent customisations or creating overwritten, orphaned entities.
    /// </summary>
    /// <param name="entitySeed">The seed that may contain an existing customisation for the navigation property.</param>
    /// <param name="getter">A lambda to the navigation property, which may include a cast, e.g. <c>x => (EmployeeAsset)x.Asset</c>.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <returns>The compatible navigation configuration, or null if none exists.</returns>
    private static INavigationSeedConfiguration? FindExistingNavigationSeeds<TEntity>(
        EntitySeed<TEntity> entitySeed,
        LambdaExpression getter)
        where TEntity : class
    {
        var property = getter.GetPropertyInfo();
        return entitySeed.Customisations
            .Where(c =>
            {
                if (c.GetterLambda is not { } existingGetter
                    || !existingGetter.GetPropertyInfo().IsSameProperty(property))
                {
                    return false;
                }

                var existingPath = existingGetter.SplitMemberAccessChain().ToList();
                var requestedPath = getter.SplitMemberAccessChain().ToList();
                return existingPath.Count == requestedPath.Count
                    && existingPath.Zip(requestedPath, (left, right) =>
                        left.GetPropertyInfo().IsSameProperty(right.GetPropertyInfo())).All(match => match);
            })
            .OfType<INavigationSeedConfiguration>()
            .FirstOrDefault(c => c.NavigationSeeds.Count > 0
                && c.NavigationSeeds.All(seed => getter.Body.Type.IsAssignableFrom(seed.EntityType)));
    }

    private sealed class NavigationParameterVisitor(ParameterExpression source, ParameterExpression destination) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == source ? destination : node;
        }
    }

    private static void AddWithDifferentForNavigationProperty<TEntity>(
        EntitySeed<TEntity> entitySeed,
        LambdaExpression left,
        IEntitySeed seed) where TEntity : class
    {
        var typeInfo = typeof(SeedDifferentNavigationPropertyConfiguration<,>).MakeGenericType(
            typeof(TEntity),
            left.Body.Type);
        object[] newArgs = [left, seed];
        var customisation = Activator.CreateInstance(typeInfo, newArgs);
        entitySeed.GetType().GetMethod(nameof(EntitySeed<>.AddCustomisation))!
            .Invoke(entitySeed, [customisation]);
    }
}