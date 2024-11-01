using System.Linq.Expressions;
using Audacia.Seed.Contracts;
using Audacia.Seed.Extensions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Properties;

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
    public PrerequisiteMatch MatchToPrerequisite(ISeedPrerequisite prerequisite)
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