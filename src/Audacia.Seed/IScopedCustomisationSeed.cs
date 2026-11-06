using Audacia.Seed.Properties;

namespace Audacia.Seed;

/// <summary>
/// A seed that can have a customisation removed, so one added for the duration of a parent's build doesn't outlive it.
/// </summary>
internal interface IScopedCustomisationSeed
{
    /// <summary>
    /// Remove the specific <paramref name="customisation"/> instance, if it was added.
    /// </summary>
    /// <param name="customisation">The customisation to remove.</param>
    void RemoveCustomisation(ISeedCustomisation customisation);
}
