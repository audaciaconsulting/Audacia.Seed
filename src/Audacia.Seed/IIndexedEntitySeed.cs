namespace Audacia.Seed;

/// <summary>
/// Builds a seed using the parent's index without requiring an invariant EntitySeed cast.
/// </summary>
internal interface IIndexedEntitySeed
{
    /// <summary>
    /// Get or create the entity for the specified index.
    /// </summary>
    /// <param name="index">The index of the entity being created.</param>
    /// <returns>The seeded entity.</returns>
    object GetOrCreateEntity(int index);
}
