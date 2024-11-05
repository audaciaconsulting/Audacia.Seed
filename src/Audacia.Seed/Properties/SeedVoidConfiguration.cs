using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

/// <summary>
/// Signpost a property that is being set by another <see cref="ISeedCustomisation{TEntity}"/>.
/// This doesn't actually set the property, but ensures we don't seed more data in the database than we need to.
/// </summary>
/// <param name="getter">A lambda to the property to signpost.</param>
/// <typeparam name="TEntity">The type of the entity containing the property.</typeparam>
internal class SeedVoidConfiguration<TEntity>(LambdaExpression getter) : ISeedCustomisation<TEntity>
    where TEntity : class
{
    /// <inheritdoc />
    public LambdaExpression GetterLambda => Getter;

    /// <summary>
    /// Gets a lambda to the property.
    /// </summary>
    private LambdaExpression Getter { get; } = getter;

    /// <inheritdoc />
    public IEntitySeed? Seed => null;

    /// <inheritdoc/>
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        // Do nothing. This exists as a marker so we don't set up prerequisites.
    }

    /// <inheritdoc/>
    public LambdaExpressionMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
    {
        ArgumentNullException.ThrowIfNull(prerequisite);

        var getter = Getter.ToNavigationProperty<TEntity>();
        return getter.MatchToPrerequisite(prerequisite);
    }

    /// <inheritdoc/>
    public IEntitySeed? FindSeedForGetter(LambdaExpression getter)
    {
        return null;
    }

    /// <inheritdoc/>
    public void Merge(ISeedCustomisation<TEntity> other)
    {
    }
}