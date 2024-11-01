using System.Diagnostics.CodeAnalysis;
using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Repositories;
using Audacia.Seed.Extensions;
using Audacia.Seed.Helpers;
using Audacia.Seed.Options;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Extensions;

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2) Seed<T1, T2>(
        this DbContext context)
        where T1 : class
        where T2 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3) Seed<T1, T2, T3>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    public static (T1 T1, T2 T2, T3 T3, T4 T4) Seed<T1, T2, T3, T4>(
        this DbContext context)
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        ArgumentNullException.ThrowIfNull(context);

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();
        var seed4 = assembly.FindSeed<T4>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();
        var seed4 = assembly.FindSeed<T4>();
        var seed5 = assembly.FindSeed<T5>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();
        var seed4 = assembly.FindSeed<T4>();
        var seed5 = assembly.FindSeed<T5>();
        var seed6 = assembly.FindSeed<T6>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();
        var seed4 = assembly.FindSeed<T4>();
        var seed5 = assembly.FindSeed<T5>();
        var seed6 = assembly.FindSeed<T6>();
        var seed7 = assembly.FindSeed<T7>();

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var assembly = EntryPointAssembly.Load();
        var seed1 = assembly.FindSeed<T1>();
        var seed2 = assembly.FindSeed<T2>();
        var seed3 = assembly.FindSeed<T3>();
        var seed4 = assembly.FindSeed<T4>();
        var seed5 = assembly.FindSeed<T5>();
        var seed6 = assembly.FindSeed<T6>();
        var seed7 = assembly.FindSeed<T7>();
        var seed8 = assembly.FindSeed<T8>();

        return context.Seed(seed1, seed2, seed3, seed4, seed5, seed6, seed7, seed8);
    }

    /// <summary>
    /// Seed the provided entity into the database context.
    /// </summary>
    /// <param name="context">The database context to insert into.</param>
    /// <param name="seed">The seed configuration that creates the entity.</param>
    /// <typeparam name="T">The type of the entity to seed.</typeparam>
    /// <returns>The saved entities in the order they were provided.</returns>
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "The method is long due to repeated code rather than complexity.")]
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
        Justification = "The method has a clear pattern and naming variables like this doesn't affect readability / understanding.")]
    [SuppressMessage("Maintainability", "AV1551: Overloaded method should call another overload.",
        Justification = "We want to save once so each method is doing everything itself.")]
    public static T Seed<T>(
        this DbContext context,
        EntitySeed<T> seed)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(seed);

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var entity = repository.PerformSeeding(seed);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2) = repository.PerformSeeding(seed1, seed2);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3) = repository.PerformSeeding(seed1, seed2, seed3);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3, entity4) = repository.PerformSeeding(seed1, seed2, seed3, seed4);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3, entity4, entity5) = repository.PerformSeeding(seed1, seed2, seed3, seed4, seed5);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3, entity4, entity5, entity6) = repository.PerformSeeding(seed1, seed2, seed3, seed4, seed5, seed6);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3, entity4, entity5, entity6, entity7) = repository.PerformSeeding(seed1, seed2, seed3, seed4, seed5, seed6, seed7);

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
    [SuppressMessage("Naming", "ACL1014: Do not include numbers in identifier name",
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var (entity1, entity2, entity3, entity4, entity5, entity6, entity7, entity8) = repository.PerformSeeding(seed1, seed2, seed3, seed4, seed5, seed6, seed7, seed8);

        return (entity1, entity2, entity3, entity4, entity5, entity6, entity7, entity8);
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        foreach (var seed in seeds)
        {
            seed.PerformSeeding(repository);
        }

        repository.Save();
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

        var repository = new EntityFrameworkCoreSeedableRepository(context);
        var entities = repository.SeedMany(amountToCreate, seed).ToList();
        repository.Save();
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
}