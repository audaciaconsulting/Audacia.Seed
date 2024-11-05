using System.Linq.Expressions;

namespace Audacia.Seed.Options;

/// <summary>
/// How well something matches to a <see cref="LambdaExpression"/>.
/// </summary>
public enum LambdaExpressionMatch
{
    /// <summary>
    /// There is no match.
    /// </summary>
    None = 0,

    /// <summary>
    /// <para>There is a partial match.</para>
    /// <para>
    /// <example>
    /// <para>
    /// <c>f => f.Foo.Bar </c> and <c>f => f.Foo.Baz</c> are not a full match, but are a partial match.
    /// </para>
    /// </example>
    /// </para>
    /// </summary>
    Partial = 100,

    /// <summary>
    /// There is a full match i.e the Lambdas are equivalent.
    /// </summary>
    Full = 200
}