using System.Linq.Expressions;
using System.Reflection;
using Audacia.Seed.Constants;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Options;

namespace Audacia.Seed.Extensions;

/// <summary>
/// Extensions for <see cref="Expression"/>s.
/// </summary>
internal static class ExpressionExtensions
{
    /// <summary>
    /// <para>
    /// Get the owner of a property inferred by a lambda. This can be the <typeparamref name="TSource"/>, or some sub-property belonging to it.
    /// </para>
    /// </summary>
    /// <example>
    /// <para>Example 1 - the owner is f.Foo:</para>
    /// <code>
    /// f => f.Foo.Bar
    /// </code>
    /// <para>Example 2 - the owner is f:</para>
    /// <code>
    /// f => f.Foo
    /// </code>
    /// </example>
    /// <param name="expression">A lambda to the property in question.</param>
    /// <param name="root">The instance to execute the lambda on to get the owner of the property.</param>
    /// <typeparam name="TSource">The type of the root object.</typeparam>
    /// <typeparam name="TProperty">The type of the property to infer.</typeparam>
    /// <returns>The owner of the property. Loosely typed as we can't easily infer it, and at the moment all we need it for is reflection.</returns>
    /// <exception cref="DataSeedingException">If we get a null-valued result.</exception>
    internal static object GetPropertyOwner<TSource, TProperty>(
        this Expression<Func<TSource, TProperty>> expression,
        TSource root)
    {
        ArgumentNullException.ThrowIfNull(expression);
        object? obj = root;
        try
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                var lambda = Expression.Lambda(memberExpression.Expression!, expression.Parameters[0]);

                obj = lambda.Compile().DynamicInvoke(root);
            }
        }
        catch (NullReferenceException)
        {
            // Do nothing. We'd rather show a more useful error (below).
        }

        return obj ?? throw new DataSeedingException(
            $"Unable to get the property owner of the provided expression {expression}. This may be because the property is being accessed via a nullable property.");
    }

    /// <summary>
    /// Get the property info from the provided lambda expression.
    /// </summary>
    /// <param name="expression">The lambda expression.</param>
    /// <returns>The property info of the getter.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="expression"/> is null.</exception>
    /// <exception cref="ArgumentException">If the expression isn't a getter to a property on the source of the lambda.</exception>
    internal static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
    {
        if (expression == null) throw new ArgumentNullException(nameof(expression));
        return expression.Body switch
        {
            null => throw new ArgumentException(
                $"The {nameof(expression.Body)} of the provided {nameof(expression)} is null"),
            UnaryExpression { Operand: MemberExpression me } => (PropertyInfo)me.Member,
            MemberExpression me => (PropertyInfo)me.Member,
            _ => throw new ArgumentException($"The expression doesn't indicate a valid property. [ {expression} ]")
        };
    }

    /// <summary>
    /// <para>
    /// Split the provided <paramref name="expression"/> into smaller expressions based on nested member access.
    /// For example, <c>x => x.Foo.Bar.Baz</c> would return <c>[x => x.Foo, x => x.Bar, x => x.Baz]</c>.
    /// </para>
    /// </summary>
    /// <param name="expression">The expression to split into many expressions.</param>
    /// <returns>An array with an item for each nested member access.</returns>
    internal static IEnumerable<LambdaExpression> SplitMemberAccessChain(
        this LambdaExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var chain = new List<LambdaExpression>();
        var currentMember = TryAddNextLambda(expression.Body, chain);

        while (currentMember != null)
        {
            currentMember = TryAddNextLambda(currentMember.Expression, chain);
        }

        chain.Reverse();
        return chain;
    }

    private static MemberExpression? TryAddNextLambda(Expression? current, List<LambdaExpression> chain)
    {
        if (current is MemberExpression memberExpression)
        {
            chain.Add(GetLambdaFromMemberExpression(memberExpression));
            return memberExpression;
        }

        if (current?.NodeType == ExpressionType.Convert &&
                 ((UnaryExpression)current).Operand is MemberExpression)
        {
            memberExpression = (MemberExpression)((UnaryExpression)current).Operand;
            chain.Add(GetLambdaFromConvertedMemberExpression(memberExpression, current.Type));
            return memberExpression;
        }

        return null;
    }

    private static LambdaExpression GetLambdaFromMemberExpression(MemberExpression memberExpression)
    {
        var param = Expression.Parameter(memberExpression.Expression!.Type, "x");
        var property = Expression.Property(param, memberExpression.Member.Name);
        var lambdaExpression = Expression.Lambda(property, param);
        return lambdaExpression;
    }

    private static LambdaExpression GetLambdaFromConvertedMemberExpression(MemberExpression memberExpression, Type convertTo)
    {
        var param = Expression.Parameter(memberExpression.Expression!.Type, "x");
        var memberAccess = Expression.MakeMemberAccess(param, memberExpression.Member);
        var explicitCast = Expression.Convert(memberAccess, convertTo);
        var lambdaExpression = Expression.Lambda(explicitCast, param);
        return lambdaExpression;
    }

    /// <summary>
    /// Join a series of member access expressions into a single expression.
    /// </summary>
    /// <param name="expressions">The expressions to join.</param>
    /// <returns>A single expression joining together the provided <paramref name="expressions"/>.</returns>
    /// <exception cref="ArgumentException">If there are any incompatabilities between neighbouring expressions.</exception>
    internal static LambdaExpression JoinMemberAccessChain(this IEnumerable<LambdaExpression> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);

        var chain = expressions.ToList();
        var lambda = chain.First();
        var memberExpression = lambda.Body;
        foreach (var currentLambda in chain.Skip(1))
        {
            memberExpression = currentLambda.Body switch
            {
                MemberExpression member => Expression.Property(memberExpression, member.Member.Name),
                UnaryExpression { Operand: MemberExpression } unary => unary,
                _ => memberExpression
            };
        }

        return Expression.Lambda(memberExpression, lambda.Parameters.First());
    }

    /// <summary>
    /// Split a member access expression into two separate expressions based on the first level up.
    /// <example>
    /// <para>
    /// For example:
    /// <c> x => x.Foo.Bar.Baz </c>
    /// becomes <c>x => x.Foo</c> and <c>x => x.Bar.Baz</c>
    /// </para>
    /// </example>
    /// </summary>
    /// <param name="expression">The expression to split.</param>
    /// <returns>Two expressions that represent the inputted <paramref name="expression"/> when combined together.</returns>
    internal static (LambdaExpression Left, LambdaExpression Right) SplitFirstMemberAccessLayer(
        this LambdaExpression expression)
    {
        var memberAccessChain = expression.SplitMemberAccessChain().ToList();
        var left = memberAccessChain[0];
        var remainingAccessChain = memberAccessChain[1..memberAccessChain.Count];
        var right = remainingAccessChain.JoinMemberAccessChain();

        return (left, right);
    }

    /// <summary>
    /// Split a member access expression into two separate expressions based on the last level up.
    /// <example>
    /// <para>
    /// For example:
    /// <c> x => x.Foo.Bar.Baz </c>
    /// becomes <c>x => x.Foo.Bar</c> and <c>x => x.Baz</c>
    /// </para>
    /// </example>
    /// </summary>
    /// <param name="expression">The expression to split.</param>
    /// <typeparam name="TRoot">The type of the parameter of the provided <paramref name="expression"/>.</typeparam>
    /// <typeparam name="TDestination">The type of the property returned by the expression.</typeparam>
    /// <returns>Two expressions that represent the inputted <paramref name="expression"/> when combined together.</returns>
    internal static (LambdaExpression Left, LambdaExpression Right) SplitLastMemberAccessLayer<TRoot, TDestination>(
        this Expression<Func<TRoot, TDestination>> expression)
    {
        var memberAccessChain = expression.SplitMemberAccessChain().ToList();
        var initialAccessChain = memberAccessChain[..^1];
        var right = memberAccessChain[^1];
        var left = initialAccessChain.JoinMemberAccessChain();

        return (left, right);
    }

    /// <summary>
    /// <para>
    /// Try to find a navigation property if this property is deemed to represent a foreign key.
    /// Otherwise, return the expression.
    /// <example>
    /// For example, passing in <c>x => x.ParentId</c> will search for a Parent property and attempt to return <c>x => x.Parent</c>.
    /// </example>
    /// </para>
    /// </summary>
    /// <param name="expression">The expression to use to find the navigation property.</param>
    /// <typeparam name="TEntity">The type of the source of the getter.</typeparam>
    /// <returns>The navigation property this foreign key represents. If this isn't a foreign key, the original expression is returned.</returns>
    /// <exception cref="ArgumentException">If the provided expression doesn't access data on <typeparamref name="TEntity"/>.</exception>
    internal static LambdaExpression ToNavigationProperty<TEntity>(
        this LambdaExpression expression)
    {
        if (expression.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException(
                $"The provided {nameof(expression)} ({expression}) does not access a property on {typeof(TEntity).Name}.");
        }

        var propertyType = expression.Body.Type;
        if (!propertyType.IsClass && memberExpression.Member.Name.EndsWith(SeedingConstants.ForeignKeySuffix))
        {
            var navigationPropertyName = memberExpression.Member.Name[..^SeedingConstants.ForeignKeySuffix.Length];
            var navigationProperty = typeof(TEntity).GetProperty(navigationPropertyName);
            if (navigationProperty != null)
            {
                var param = Expression.Parameter(memberExpression.Expression!.Type, "x");
                expression = Expression.Lambda(Expression.MakeMemberAccess(param, navigationProperty), param);
            }
        }

        return expression;
    }

    /// <summary>
    /// <para>
    /// Try to find a navigation property if this property is deemed to represent a foreign key.
    /// Otherwise, return the expression.
    /// <example>
    /// For example, passing in <c>x => x.ParentId</c> will search for a Parent property and attempt to return <c>x => x.Parent</c>.
    /// </example>
    /// </para>
    /// </summary>
    /// <param name="expression">The expression to use to find the navigation property.</param>
    /// <typeparam name="TEntity">The type of the source of the getter.</typeparam>
    /// <typeparam name="TProperty">The type of the destination of the getter.</typeparam>
    /// <returns>The navigation property this foreign key represents. If this isn't a foreign key, the original expression is returned.</returns>
    /// <exception cref="ArgumentException">If the provided expression doesn't access data on <typeparamref name="TEntity"/>.</exception>
    internal static LambdaExpression ToNavigationProperty<TEntity, TProperty>(
        this Expression<Func<TEntity, TProperty>> expression)
    {
        return expression.ToNavigationProperty<TEntity>();
    }

    /// <summary>
    /// Match the <paramref name="expression"/> to the provided <paramref name="prerequisite"/>.
    /// </summary>
    /// <param name="expression">The expression to match.</param>
    /// <param name="prerequisite">The prerequisite to compare to.</param>
    /// <returns>A value indicating how well they match.</returns>
    internal static LambdaExpressionMatch MatchToPrerequisite(
        this LambdaExpression expression,
        ISeedPrerequisite prerequisite)
    {
        var propertyInfo = expression.GetPropertyInfo();
        if (prerequisite.PropertyInfo == propertyInfo)
        {
            return LambdaExpressionMatch.Full;
        }

        // Check if they have any commonality in their member access chain.
        if (expression.SplitMemberAccessChain().Count() > 1)
        {
            var (left, _) = expression.SplitFirstMemberAccessLayer();
            if (prerequisite.PropertyInfo == left.GetPropertyInfo())
            {
                return LambdaExpressionMatch.Partial;
            }
        }

        return LambdaExpressionMatch.None;
    }
}