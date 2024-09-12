using System.Diagnostics.CodeAnalysis;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;

namespace Audacia.Seed.InMemory;

/// <summary>
/// Helper class to build entities in-memory.
/// </summary>
public class EntityBuilder
{
    private readonly InMemorySeedableRepository _repo = new();

    /// <summary>
    /// Seed the provided entity into the database context.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public TEntity Build<TEntity>()
        where TEntity : class
    {
        var seed = EntryPointAssembly.Load().FindSeed<TEntity>();

        return Build(seed);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2) Build<T1, T2>()
        where T1 : class
        where T2 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();

        return Build(
            seed1, seed2);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3) Build<T1, T2, T3>()
        where T1 : class
        where T2 : class
        where T3 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();

        return Build(
            seed1, seed2, seed3);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4) Build<T1, T2, T3, T4>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();

        return Build(
            seed1, seed2, seed3, seed4);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5) Build<T1, T2, T3, T4, T5>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();

        return Build(
            seed1, seed2, seed3, seed4, seed5);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6) Build<T1, T2, T3, T4, T5, T6>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();

        return Build(seed1, seed2, seed3, seed4, seed5, seed6);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7) Build<T1, T2, T3, T4, T5, T6, T7>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();
        var seed7 = EntryPointAssembly.Load().FindSeed<T7>();

        return Build(
            seed1, seed2, seed3, seed4, seed5, seed6, seed7);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <typeparam name="T1">The type of the first entity to seed.</typeparam>
    /// <typeparam name="T2">The type of the second entity to seed.</typeparam>
    /// <typeparam name="T3">The type of the third entity to seed.</typeparam>
    /// <typeparam name="T4">The type of the fourth entity to seed.</typeparam>
    /// <typeparam name="T5">The type of the fifth entity to seed.</typeparam>
    /// <typeparam name="T6">The type of the sixth entity to seed.</typeparam>
    /// <typeparam name="T7">The type of the seventh entity to seed.</typeparam>
    /// <typeparam name="T8">The type of the eighth entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7, T8 T8) Build<T1, T2, T3, T4, T5, T6, T7, T8>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        var seed1 = EntryPointAssembly.Load().FindSeed<T1>();
        var seed2 = EntryPointAssembly.Load().FindSeed<T2>();
        var seed3 = EntryPointAssembly.Load().FindSeed<T3>();
        var seed4 = EntryPointAssembly.Load().FindSeed<T4>();
        var seed5 = EntryPointAssembly.Load().FindSeed<T5>();
        var seed6 = EntryPointAssembly.Load().FindSeed<T6>();
        var seed7 = EntryPointAssembly.Load().FindSeed<T7>();
        var seed8 = EntryPointAssembly.Load().FindSeed<T8>();

        return Build(
            seed1, seed2, seed3, seed4, seed5, seed6, seed7, seed8);
    }

    /// <summary>
    /// Seeds a single entity and immediately return it.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <param name="seed">The seed configuration that creates the entity.</param>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    /// <exception cref="DataSeedingException">Thrown if the number of entities added is not exactly 1.</exception>
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public TEntity Build<TEntity>(EntitySeed<TEntity> seed)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(seed);

        seed.PrepareToSeed(_repo);
        var entity = seed.Build();

        _repo.Add(entity);

        return entity;
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public (T1 T1, T2 T2) Build<T1, T2>(
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2)
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();

        return (entity1, entity2);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
    /// <param name="seed1">The first seed to add.</param>
    /// <param name="seed2">The second seed to add.</param>
    /// <param name="seed3">The third seed to add.</param>
    /// <typeparam name="T1">The type of the first seed.</typeparam>
    /// <typeparam name="T2">The type of the second seed.</typeparam>
    /// <typeparam name="T3">The type of the third seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public (T1 T1, T2 T2, T3 T3) Build<T1, T2, T3>(
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();

        return (entity1, entity2, entity3);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage(
        "Maintainability",
        "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4) Build<T1, T2, T3, T4>(
        EntitySeed<T1> seed1,
        EntitySeed<T2> seed2,
        EntitySeed<T3> seed3,
        EntitySeed<T4> seed4)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        seed4.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();
        var entity4 = seed4.Build();

        return (entity1, entity2, entity3, entity4);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage(
        "Maintainability",
        "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5) Build<T1, T2, T3, T4, T5>(
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
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        seed4.PrepareToSeed(_repo);
        seed5.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();
        var entity4 = seed4.Build();
        var entity5 = seed5.Build();

        return (entity1, entity2, entity3, entity4, entity5);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage(
        "Maintainability",
        "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6) Build<T1, T2, T3, T4, T5, T6>(
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
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        seed4.PrepareToSeed(_repo);
        seed5.PrepareToSeed(_repo);
        seed6.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();
        var entity4 = seed4.Build();
        var entity5 = seed5.Build();
        var entity6 = seed6.Build();

        return (entity1, entity2, entity3, entity4, entity5, entity6);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage(
        "Maintainability",
        "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7) Build<T1, T2, T3, T4, T5, T6, T7>(
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
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        seed4.PrepareToSeed(_repo);
        seed5.PrepareToSeed(_repo);
        seed6.PrepareToSeed(_repo);
        seed7.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();
        var entity4 = seed4.Build();
        var entity5 = seed5.Build();
        var entity6 = seed6.Build();
        var entity7 = seed7.Build();

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7);
    }

    /// <summary>
    /// Seed the provided entities into the database context.
    /// </summary>
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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "Naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    [SuppressMessage(
        "Maintainability",
        "ACL1003: Don't declare signatures with more than a predefined number of parameters.",
        Justification = "The parameters are essentially array params, but we want to return the actual type.")]
    public (T1 T1, T2 T2, T3 T3, T4 T4, T5 T5, T6 T6, T7 T7, T8 T8) Build<T1, T2, T3, T4, T5, T6, T7, T8>(
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
        ArgumentNullException.ThrowIfNull(seed1);
        ArgumentNullException.ThrowIfNull(seed2);
        ArgumentNullException.ThrowIfNull(seed3);
        ArgumentNullException.ThrowIfNull(seed4);
        ArgumentNullException.ThrowIfNull(seed5);
        ArgumentNullException.ThrowIfNull(seed6);
        ArgumentNullException.ThrowIfNull(seed7);
        ArgumentNullException.ThrowIfNull(seed8);

        seed1.PrepareToSeed(_repo);
        seed2.PrepareToSeed(_repo);
        seed3.PrepareToSeed(_repo);
        seed4.PrepareToSeed(_repo);
        seed5.PrepareToSeed(_repo);
        seed6.PrepareToSeed(_repo);
        seed7.PrepareToSeed(_repo);
        seed8.PrepareToSeed(_repo);
        var entity1 = seed1.Build();
        var entity2 = seed2.Build();
        var entity3 = seed3.Build();
        var entity4 = seed4.Build();
        var entity5 = seed5.Build();
        var entity6 = seed6.Build();
        var entity7 = seed7.Build();
        var entity8 = seed8.Build();

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7, entity8);
    }

    /// <summary>
    /// Seed many entities and immediately return them.
    /// </summary>
    /// <param name="amountToCreate">The amount of entities to create.</param>
    /// <param name="seed">The seed configuration that creates the entity.</param>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public virtual IEnumerable<TEntity> BuildMany<TEntity>(
        int amountToCreate,
        EntitySeed<TEntity> seed)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(seed);

        seed.Repository = _repo;
        var entities = seed.BuildMany(amountToCreate).ToList();

        foreach (var entity in entities)
        {
            seed.Repository!.Add(entity);
        }

        return entities;
    }

    /// <summary>
    /// Seed many entities and immediately return them.
    /// </summary>
    /// <param name="amountToCreate">The amount of entities to create.</param>
    /// <typeparam name="TEntity">The type of the entity to seed.</typeparam>
    /// <returns>An entity that exists in the database, with a generated ID.</returns>
    public IEnumerable<TEntity> BuildMany<TEntity>(
        int amountToCreate)
        where TEntity : class
    {
        var seed = EntryPointAssembly.Load().FindSeed<TEntity>();

        return BuildMany(amountToCreate, seed);
    }
}