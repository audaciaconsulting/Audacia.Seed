using System.Collections.Concurrent;

namespace Audacia.Seed.Helpers;

/// <summary>
/// Cached information for a type to prevent expensive recalculations.
/// </summary>
internal static class TypeCaches
{
    /// <summary>
    /// Gets a cache of seed types and their prerequisites.
    /// </summary>
    internal static ConcurrentDictionary<Type, IEnumerable<ISeedPrerequisite>> Prerequisites { get; } = new();

    /// <summary>
    /// Gets a cache of seed types and the type of their corresponding entity seed.
    /// Cache the type because the entity seed class contains objects that are disposed.
    /// </summary>
    internal static ConcurrentDictionary<Type, Type> SeedClassTypes { get; } = new();
}