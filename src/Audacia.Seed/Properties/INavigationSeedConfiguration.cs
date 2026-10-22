using Audacia.Seed.Customisation;
using Audacia.Seed.Exceptions;
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

    /// <summary>
    /// Protect against using `WithDifferent` when you're only creating one entity.
    /// </summary>
    /// <param name="entitySeed">The seed for the entity which owns the navigation property.</param>
    /// <typeparam name="TEntity">The type of the entity being seeded.</typeparam>
    /// <exception cref="DataSeedingException">If only one <typeparamref name="TEntity"/> is being built.</exception>
    internal static void ValidateDifferentSeeds<TEntity>(EntitySeed<TEntity> entitySeed)
        where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(entitySeed);

        // As `TryFindNew` is the default, this is what the seed options would look like in this scenario.
        if (entitySeed.Options is { AmountToCreate: 1, InsertionBehavior: SeedingInsertionBehaviour.TryFindNew })
        {
            throw new DataSeedingException($"`{nameof(EntitySeedExtensions.WithDifferent)} was specified, but we're only building 1 {typeof(TEntity).Name}.");
        }
    }
}
