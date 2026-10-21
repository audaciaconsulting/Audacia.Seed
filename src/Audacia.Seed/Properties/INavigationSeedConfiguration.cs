using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Provides the navigation seeds that can be augmented without replacing their customisation.
/// </summary>
internal interface INavigationSeedConfiguration
{
    /// <summary>
    /// Gets all seeds, in the order in which they will be used.
    /// </summary>
    IReadOnlyList<IEntitySeed> NavigationSeeds { get; }

    /// <summary>
    /// Ensure each entity receives a new navigation entity.
    /// </summary>
    void UseDifferentSeeds()
    {
        foreach (var seed in NavigationSeeds)
        {
            seed.Options.InsertionBehavior = SeedingInsertionBehaviour.AddNew;
        }
    }
}
