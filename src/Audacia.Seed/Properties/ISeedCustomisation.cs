using System.Linq.Expressions;
using Audacia.Seed.Contracts;

namespace Audacia.Seed.Properties;

/// <summary>
/// Interface for populating a property on an entity to override default behaviour.
/// </summary>
/// <typeparam name="TEntity">The type of entity that the property belongs to.</typeparam>
public interface ISeedCustomisation<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Apply the customisation on the entity.
    /// </summary>
    /// <param name="entity">The entity to populate the property on. Not necessarily the owner of the property e.g if the provided expression is f => f.Foo.Bar.</param>
    /// <param name="repository">For looking up existing information persisted in the repository, if necessary.</param>
    /// <param name="index">The index of this specific entity being created. Use this to vary how properties are set based on how many entities are already seeded.</param>
    /// <param name="previous">The previous entity that was created - does not exist in the database. Use this to vary how properties are set based what the previous entity looks like.</param>
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous);

    /// <summary>
    /// Return a value indicating whether this customisation is for the same property as the provided prerequisite.
    /// </summary>
    /// <param name="prerequisite">The prerequisite to compare to.</param>
    /// <returns>True if the prerequisite is for the same property at this. Otherwise false.</returns>
    bool EqualsPrerequisite(ISeedPrerequisite prerequisite) => false;

    /// <summary>
    /// Return a predicate that would match a <typeparamref name="TEntity"/> to the cusomisation this represents.
    /// </summary>
    /// <returns>A predicate to match to.</returns>
    Expression<Func<TEntity, bool>> ToPredicate() => _ => true;

    /// <summary>
    /// Ensure the setup of the customisation is valid.
    /// </summary>
    /// <param name="entitySeed">The seed to validate against.</param>
    void Validate(EntitySeed<TEntity> entitySeed)
    {
    }

    /// <summary>
    /// Gets a value indicating the order in which this customisation should be applied.
    /// The lower the number, the higher the priority.
    /// </summary>
    int Order => 0;
}