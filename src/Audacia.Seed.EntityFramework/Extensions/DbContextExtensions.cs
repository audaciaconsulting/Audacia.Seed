using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFramework.Repositories;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;
using Audacia.Seed.Options;

namespace Audacia.Seed.EntityFramework.Extensions;

/// <summary>
/// Extensions for <see cref="DbContext"/>s.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Seed the provided entity into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public static TEntity Seed<TEntity>(this DbContext context)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed = EntryPointAssembly.Load().FindSeed<TEntity>();

        return context.Seed(seed);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2) Seed<T1, T2>(
        this DbContext context)
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();

        return context.Seed(seed1, seed2);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3) Seed<T1, T2, T3>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();

        return context.Seed(seed1, seed2, seed3);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4) Seed<T1, T2, T3, T4>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();

        return context.Seed(seed1, seed2, seed3, seed4);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5) Seed<T1, T2, T3, T4, T5>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();

        return context.Seed(seed1, seed2, seed3, seed4, seed5);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6) Seed<T1, T2, T3, T4, T5, T6>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();

        return context.Seed(seed1, seed2, seed3, seed4, seed5, seed6);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7) Seed<T1, T2, T3, T4, T5, T6, T7>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();
        var seed7 = EntryPointAssembly.Load().FindSeed<T7>();

        return context.Seed(seed1, seed2, seed3, seed4, seed5, seed6, seed7);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh entity to seed.</typeparam>
    /// <typeparam name="T8">The type of the eighth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7, T8 T8) Seed<T1, T2, T3, T4, T5, T6, T7, T8>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();
        var seed7 = EntryPointAssembly.Load().FindSeed<T7>();
        var seed8 = EntryPointAssembly.Load().FindSeed<T8>();

        return context.Seed(seed1, seed2, seed3, seed4, seed5, seed6, seed7, seed8);
    }

    /// <summary>
    /// Seeds a single entity and immediately return it.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <param name="context">The database context to insert into.</param>
    /// <param name="seed">The seed configuration that creates the entity.</param>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    /// <exception cref="DataSeedingException">Thrown if the number of entities added is not exactly 1.</exception>
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public static TEntity Seed<TEntity>(
        this DbContext context,
        EntitySeed<TEntity> seed)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed.PrepareToSeed(repository);
        var entity = seed.Build();

        repository.Add(entity);
        context.PerformSave();

        return entity;
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("Naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public static (T1 T1, T2 T2) Seed<T1, T2>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2)
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);

        context.PerformSave();

        return (entity1, entity2);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public static (T1 T1, T2 T2, T3 T3) Seed<T1, T2, T3>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);

        context.PerformSave();

        return (entity1, entity2, entity3);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <param name="seed4">The fourth seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage("Maintainability", "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4) Seed<T1, T2, T3, T4>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        seed4.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);
        var entity4 = seed4.Build();
        repository.Add(entity4);

        context.PerformSave();

        return (entity1, entity2, entity3, entity4);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <param name="seed4">The fourth seed to add.</param>
    /// <param name="seed5">The fifth seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage("Maintainability", "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5) Seed<T1, T2, T3, T4, T5>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4,
        EntitySeed<T5> seed5)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        seed4.PrepareToSeed(repository);
        seed5.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);
        var entity4 = seed4.Build();
        repository.Add(entity4);
        var entity5 = seed5.Build();
        repository.Add(entity5);

        context.PerformSave();

        return (entity1, entity2, entity3, entity4, entity5);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <param name="seed4">The fourth seed to add.</param>
    /// <param name="seed5">The fifth seed to add.</param>
    /// <param name="seed6">The sixth seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage("Maintainability", "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6) Seed<T1, T2, T3, T4, T5, T6>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4,
        EntitySeed<T5> seed5,
        EntitySeed<T6> seed6)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        seed4.PrepareToSeed(repository);
        seed5.PrepareToSeed(repository);
        seed6.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);
        var entity4 = seed4.Build();
        repository.Add(entity4);
        var entity5 = seed5.Build();
        repository.Add(entity5);
        var entity6 = seed6.Build();
        repository.Add(entity6);

        context.PerformSave();

        return (entity1, entity2, entity3, entity4, entity5, entity6);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <param name="seed4">The fourth seed to add.</param>
    /// <param name="seed5">The fifth seed to add.</param>
    /// <param name="seed6">The sixth seed to add.</param>
    /// <param name="seed7">The seventh seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage("Maintainability", "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7) Seed<T1, T2, T3, T4, T5, T6, T7>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4,
        EntitySeed<T5> seed5,
        EntitySeed<T6> seed6,
        EntitySeed<T7> seed7)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        seed4.PrepareToSeed(repository);
        seed5.PrepareToSeed(repository);
        seed6.PrepareToSeed(repository);
        seed7.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);
        var entity4 = seed4.Build();
        repository.Add(entity4);
        var entity5 = seed5.Build();
        repository.Add(entity5);
        var entity6 = seed6.Build();
        repository.Add(entity6);
        var entity7 = seed7.Build();
        repository.Add(entity7);

        context.PerformSave();

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="context">The database context to seed the entities into.</param>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <param name="seed4">The fourth seed to add.</param>
    /// <param name="seed5">The fifth seed to add.</param>
    /// <param name="seed6">The sixth seed to add.</param>
    /// <param name="seed7">The seventh seed to add.</param>
    /// <param name="seed8">The eighth seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh seed.</typeparam>
    /// <typeparam name="T8">The type of the eighth seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("naming-conventions", "AV1704: Don't include numbers in variables, parameters and type members",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage("Maintainability", "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7, T8 T8) Seed<T1, T2, T3, T4, T5, T6, T7, T8>(
        this DbContext context,
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4,
        EntitySeed<T5> seed5,
        EntitySeed<T6> seed6,
        EntitySeed<T7> seed7,
        EntitySeed<T8> seed8)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);
        ArgumentNullException.ThrowIfNull(seed8);

        var repository = new EntityFrameworkSeedableRepository(context);
        seed1.PrepareToSeed(repository);
        seed2.PrepareToSeed(repository);
        seed3.PrepareToSeed(repository);
        seed4.PrepareToSeed(repository);
        seed5.PrepareToSeed(repository);
        seed6.PrepareToSeed(repository);
        seed7.PrepareToSeed(repository);
        seed8.PrepareToSeed(repository);
        var entity1 = seed1.Build();
        repository.Add(entity1);
        var entity2 = seed2.Build();
        repository.Add(entity2);
        var entity3 = seed3.Build();
        repository.Add(entity3);
        var entity4 = seed4.Build();
        repository.Add(entity4);
        var entity5 = seed5.Build();
        repository.Add(entity5);
        var entity6 = seed6.Build();
        repository.Add(entity6);
        var entity7 = seed7.Build();
        repository.Add(entity7);
        var entity8 = seed8.Build();
        repository.Add(entity8);

        context.PerformSave();

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7, entity8);
    }

    /// <summary>
    /// Seed many entities and immediately return them.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <param name="amountToCreate">The amount of entities to create.</param>
    /// <param name="seed">The seed configuration that creates the entity.</param>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public static IEnumerable<TEntity> SeedMany<TEntity>(
        this DbContext context,
        int amountToCreate,
        EntitySeed<TEntity> seed)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed);

        seed.Repository = new EntityFrameworkSeedableRepository(context);
        var entities = seed.BuildMany(amountToCreate).ToList();

        foreach (var entity in entities)
        {
            seed.Repository!.Add(entity);
        }

        context.PerformSave();

        return entities;
    }

    /// <summary>
    /// Seed many entities and immediately return them.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <param name="amountToCreate">The amount of entities to create.</param>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public static IEnumerable<TEntity> SeedMany<TEntity>(
        this DbContext context,
        int amountToCreate)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var seed = EntryPointAssembly.Load().FindSeed<TEntity>();

        return context.SeedMany(amountToCreate, seed);
    }

    /// <summary>
    /// Seed many entities and immediately return it.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <param name="seeds">The entity seed configurations.</param>
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public static void Seed(
        this DbContext context,
        params IEntitySeed[] seeds)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seeds);

        foreach (var seed in seeds)
        {
            if (seed.Options.InsertionBehavior != SeedingInsertionBehaviour.MustFindExisting)
            {
                seed.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
            }

            seed.PerformSeeding(new EntityFrameworkSeedableRepository(context));
        }

        context.PerformSave();
    }

    private static void PerformSave(this DbContext context)
    {
        try
        {
            context.SaveChanges();
        }
        catch (DbEntityValidationException exception)
        {
            throw new DataSeedingException(exception.GetErrors());
        }
    }
}