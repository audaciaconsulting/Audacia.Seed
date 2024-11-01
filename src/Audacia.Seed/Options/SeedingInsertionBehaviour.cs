namespace Audacia.Seed.Options;

/// <summary>
/// The different behaviours we can use when inserting an entity into the database.
/// </summary>
public enum SeedingInsertionBehaviour
{
    /// <summary>
    /// The default behaviour, which is to try and find an entity in the change tracker (whether it's been saved or not).
    /// <br/>
    /// If it can't find one, we'll fall back on the behaviour described by <see cref="AddNew"/>.
    /// </summary>
    TryFindNew = 0,

    /// <summary>
    /// We will always add a new entity to the database.
    /// </summary>
    AddNew = 100,

    /// <summary>
    /// Try to match up with an already saved entity (not in the change tracker).
    /// <br/>
    /// Use this if your seed has hardcoded IDs to prevent duplicates.
    /// <br/>
    /// If it can't find one, we'll fall back on the behaviour described by <see cref="TryFindNew"/>.
    /// </summary>
    TryFindExisting = 200,
}