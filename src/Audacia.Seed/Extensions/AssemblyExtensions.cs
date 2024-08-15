using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Audacia.Seed.Attributes;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Helpers;

namespace Audacia.Seed.Extensions;

/// <summary>
/// Extensions for the <see cref="Assembly"/>.
/// </summary>
internal static class AssemblyExtensions
{
    /// <summary>
    /// Find an appropriate seed class for the given entity type, falling back on a <see cref="EntitySeed{TEntity}"/> if none can be found.
    /// <br/>
    /// This will return the first one it finds in the case of there existing multiple suitable seed classes.
    /// </summary>
    /// <param name="assembly">The assembly to look for an <see cref="EntitySeed{TEntity}"/> in.</param>
    /// <typeparam name="TEntity">The type of entity we're finding a seed class for.</typeparam>
    /// <returns>A seed for <typeparamref name="TEntity"/>.</returns>
    /// <exception cref="DataSeedingException">If we can't load the seed assembly.</exception>
    [SuppressMessage("Maintainability", "AV1551: Call the more overloaded method from other overloads", Justification = "Would prefer to return the implementation type.")]
    public static EntitySeed<TEntity> FindSeed<TEntity>(this Assembly assembly)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(assembly);

        if (TypeCaches.SeedClassTypes.ContainsKey(typeof(TEntity)))
        {
            var seedClassType = TypeCaches.SeedClassTypes[typeof(TEntity)];
            return (EntitySeed<TEntity>)Activator.CreateInstance(seedClassType)!;
        }

        var seedAssemblies = assembly.GetCustomAttributes<SeedAssemblyAttribute>()
            .Select(seedAssemblyAttribute => Assembly.Load(seedAssemblyAttribute.Name) ?? throw new DataSeedingException($"Unable to load assembly {seedAssemblyAttribute.Name}. Ensure it is referenced in the project."))
            .ToList();
        if (!seedAssemblies.Any())
        {
            seedAssemblies = [assembly];
        }

        var seedConfiguration = seedAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(EntitySeed<TEntity>)))
            .Select(Activator.CreateInstance)
            .Cast<EntitySeed<TEntity>>()
            .FirstOrDefault() ?? new EntitySeed<TEntity>();

        var seedConfigurationType = seedConfiguration.GetType();
        TypeCaches.SeedClassTypes.AddOrUpdate(typeof(TEntity), seedConfigurationType, (_, _) => seedConfigurationType);

        return seedConfiguration;
    }

    /// <summary>
    /// Find an appropriate seed class for the given entity type, falling back on a <see cref="EntitySeed{TEntity}"/> if none can be found.
    /// <br/>
    /// This will return the first one it finds in the case of there existing multiple suitable seed classes.
    /// </summary>
    /// <param name="assembly">The assembly to look for an <see cref="EntitySeed{TEntity}"/> in.</param>
    /// <param name="entityType">The type of the enttiy to find a seed for.</param>
    /// <returns>A seed for the <paramref name="entityType"/>.</returns>
    /// <exception cref="DataSeedingException">If we can't load the seed assembly.</exception>
    public static IEntitySeed FindSeed(this Assembly assembly, Type entityType)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var seedAssemblies = assembly.GetCustomAttributes<SeedAssemblyAttribute>()
            .Select(seedAssemblyAttribute => Assembly.Load(seedAssemblyAttribute.Name) ?? throw new DataSeedingException($"Unable to load assembly {seedAssemblyAttribute.Name}. Ensure it is referenced in the project."))
            .ToList();
        if (!seedAssemblies.Any())
        {
            seedAssemblies = [assembly];
        }

        var seedConfiguration = seedAssemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(EntitySeed<>).MakeGenericType(entityType)))
            .Select(Activator.CreateInstance)
            .Cast<IEntitySeed>()
            .FirstOrDefault();

        if (seedConfiguration == null)
        {
            var defaultEntitySeedType = typeof(EntitySeed<>);
            var genericType = defaultEntitySeedType.MakeGenericType(entityType);
            // Fallback on the default seed
            seedConfiguration = (IEntitySeed)Activator.CreateInstance(genericType)!;
        }

        return seedConfiguration;
    }
}