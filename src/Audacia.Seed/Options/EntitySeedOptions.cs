namespace Audacia.Seed.Options;

/// <summary>
/// Options for how we can seed a specific <see cref="IEntitySeed"/>.
/// </summary>
public record EntitySeedOptions
{
    private const int DefaultAmountToCreate = 1;

    /// <summary>
    /// Gets or sets the numbers of entities to seed. Defaults to 1.
    /// </summary>
    public int AmountToCreate { get; set; } = DefaultAmountToCreate;

    /// <summary>
    /// Gets or sets how we'll seed this entity into the repository.
    /// </summary>
    public SeedingInsertionBehaviour InsertionBehavior { get; set; }

    /// <summary>
    /// Merge this and the provided <paramref name="options"/> together.
    /// </summary>
    /// <param name="options">The other options to merge in.</param>
    public void Merge(EntitySeedOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        // Take the value that isn't the default, if the other specifies.
        // This is because we should prioritise behaviour that isn't the default as it means this information was explicitly set.
        if (AmountToCreate == DefaultAmountToCreate && options.AmountToCreate != DefaultAmountToCreate)
        {
            AmountToCreate = options.AmountToCreate;
        }

        if (InsertionBehavior == default && options.InsertionBehavior != default)
        {
            InsertionBehavior = options.InsertionBehavior;
        }
    }
}