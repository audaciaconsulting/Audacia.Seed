using Audacia.Seed.Contracts;
using Audacia.Seed.Options;
using Audacia.Seed.Properties;

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
    /// Gets or sets the destination of where this data will be seeded.
    /// This is used to look up data that may already exist in the repository.
    /// </summary>
    ISeedableRepository? Repository { get; set; }

    /// <summary>
    /// Gets the options for how we will seed this entity.
    /// </summary>
    EntitySeedOptions Options { get; }

    /// <summary>
    /// Gets a value indicating whether this entity seed has customisations applied.
    /// </summary>
    bool HasCustomisations { get; }

    /// <summary>
    /// Gets or sets the entities that must be seeded as a prerequisite to this.
    /// <br/>
    /// This should be kept as succinct as possible, and seeding prerequisites for optional navigation properties should be avoided.
    /// </summary>
    /// <returns>An enumerable of prerequisites that will be seeded before this.</returns>
    IEnumerable<ISeedPrerequisite> Prerequisites();

    /// <summary>
    /// Seed this entity into the provided <paramref name="seedableRepository"/>.
    /// </summary>
    /// <param name="seedableRepository">The repository to seed the entity into.</param>
    void PerformSeeding(ISeedableRepository seedableRepository);

    void AddCustomisation(ISeedCustomisation customisation);
}

/// <summary>
/// Configuration for a single entity to be seeded into the database.
/// <br/>
/// This should seed exactly what is needed, with implementations differing on a per use case basis.
/// </summary>
/// <typeparam name="TEntity">The type of object we're seeding.</typeparam>
public interface IEntitySeed<out TEntity> : IEntitySeed
    where TEntity : class
{
    /// <summary>
    /// Build a <typeparamref name="TEntity"/> based on the provided customisations, so we can seed it.
    /// </summary>
    /// <returns>The entity as specified in the seed configuration.</returns>
    internal TEntity Build();
}