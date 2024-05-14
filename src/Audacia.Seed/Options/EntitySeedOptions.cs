namespace Audacia.Seed.Options;

/// <summary>
/// Options for how we can seed a specific <see cref="IEntitySeed"/>.
/// </summary>
public record EntitySeedOptions
{
    /// <summary>
    /// Gets or sets the numbers of entities to seed. Defaults to 1.
    /// </summary>
    public int AmountToCreate { get; set; } = 1;

    /// <summary>
    /// Gets or sets how we'll seed this entity into the repository.
    /// </summary>
    public SeedingInsertionBehaviour InsertionBehavior { get; set; }
}