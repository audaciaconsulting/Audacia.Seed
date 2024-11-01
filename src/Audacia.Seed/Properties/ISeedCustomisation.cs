using System.Linq.Expressions;
using Audacia.Seed.Constants;
using Audacia.Seed.Contracts;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Interface for populating a property on an entity to override default behaviour.
/// </summary>
/// <typeparam name="TEntity">The type of entity that the property belongs to.</typeparam>
public interface ISeedCustomisation<TEntity> : ISeedCustomisation
    where TEntity : class
{
    /// <summary>
    /// Apply the customisation on the entity.
    /// </summary>
    /// <param name="entity">The entity to apply the customisation to.</param>
    /// <param name="repository">For looking up existing information persisted in the repository, if necessary.</param>
    /// <param name="index">The index of this specific entity being created. Use this to vary how properties are set based on how many entities are already seeded.</param>
    /// <param name="previous">The previous entity that was created - does not exist in the database. Use this to vary how properties are set based what the previous entity looks like.</param>
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous);

    /// <summary>
    /// Return a predicate that would match a <typeparamref name="TEntity"/> to the customisation this represents.
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
    /// Merge this customisation with another.
    /// </summary>
    /// <param name="other">The other customisation to merge in.</param>
    void Merge(ISeedCustomisation<TEntity> other);
}

public interface ISeedCustomisation
{
    /// <summary>
    /// Gets a value indicating the order in which this customisation should be applied.
    /// The lower the number, the higher the priority.
    /// Defaults to 100 so that we can insert customisations at the start or end of the pipeline.
    /// </summary>
    int Order => SeedingConstants.DefaultCustomisationOrder;

    /// <summary>
    /// Based on the provided <paramref name="getter"/>, return a <see cref="EntitySeed{TNavigation}"/>.
    /// </summary>
    /// <param name="getter">An expression lambda to the property being customised.</param>
    /// <returns>A seed configuration if the getter represents the same member as this. Otherwise null.</returns>
    IEntitySeed? FindSeedForGetter(LambdaExpression getter);

    /// <summary>
    /// Return a value indicating how well this customisation matches the same property as the provided prerequisite.
    /// </summary>
    /// <param name="prerequisite">The prerequisite to compare to.</param>
    /// <returns></returns>
    PrerequisiteMatch MatchToPrerequisite(ISeedPrerequisite prerequisite);

    LambdaExpression? GetterLambda { get; }

    IEntitySeed? Seed { get; }
}