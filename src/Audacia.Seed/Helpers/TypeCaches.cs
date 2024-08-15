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
}