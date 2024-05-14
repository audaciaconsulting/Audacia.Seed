using Audacia.Seed.Exceptions;

namespace Audacia.Seed.Options;

/// <summary>
/// The different behaviours we can use when inserting an entity into the database.
/// </summary>
public enum SeedingInsertionBehaviour
{
    /// <summary>
    /// The default behaviour, which is to find an existing entity if possible, or create a new one if not.
    /// </summary>
    TryFindExisting = 0,

    /// <summary>
    /// We will always add a new entity to the database.
    /// </summary>
    AddNew = 100,

    /// <summary>
    /// Find an existing entity, or throw a <see cref="DataSeedingException"/> if we cannot find one.
    /// <br/>
    /// Use this if your seed has hardcoded IDs to prevent duplicates.
    /// </summary>
    MustFindExisting = 200,
}