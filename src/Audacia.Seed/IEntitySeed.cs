using Audacia.Seed.Contracts;
using Audacia.Seed.Options;

namespace Audacia.Seed;

/// <summary>
/// Configuration for a single entity to be seeded into the database.
/// <br/>
/// This should seed exactly what is needed, with implementations differing on a per use case basis.
/// </summary>
public interface IEntitySeed
{
    /// <summary>
    /// Gets the type of the entity being seeded.
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// Gets the options for how we will seed this entity.
    /// </summary>
    EntitySeedOptions Options { get; }

    /// <summary>
    /// Gets or sets the entities that must be seeded as a prerequisite to this.
    /// <br/>
    /// This should be kept as succinct as possible, and seeding prerequisites for optional navigation properties should be avoided.
    /// </summary>
    /// <returns>An enumerable of prerequisites that will be seeded before this.</returns>
    public IEnumerable<ISeedPrerequisite> Prerequisites();

    /// <summary>
    /// Seed this entity into the provided <paramref name="seedableRepository"/>.
    /// </summary>
    /// <param name="seedableRepository">The repository to seed the entity into.</param>
    void PerformSeeding(ISeedableRepository seedableRepository);
}