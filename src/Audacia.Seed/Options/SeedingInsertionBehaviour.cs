namespace Audacia.Seed.Options;

/// <summary>
/// The different behaviours we can use when inserting an entity into the database.
/// </summary>
public enum SeedingInsertionBehaviour
{
    /// <summary>
    /// The default behaviour, which is to find an entity in the repository that has not yet been saved, or create a new one if not.
    /// </summary>
    TryFindNew = 0,

    /// <summary>
    /// We will always add a new entity to the database.
    /// </summary>
    AddNew = 100,

    /// <summary>
    /// Like <see cref="TryFindNew"/>, but will also check if it can match up with an already saved entity.
    /// <br/>
    /// Use this if your seed has hardcoded IDs to prevent duplicates.
    /// </summary>
    TryFindExisting = 200,
}