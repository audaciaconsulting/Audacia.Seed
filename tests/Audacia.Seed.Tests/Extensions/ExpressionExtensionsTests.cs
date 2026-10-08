using System.Linq.Expressions;
using Audacia.Seed.Extensions;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Shouldly;
using Xunit;

namespace Audacia.Seed.Tests.Extensions;

public class ExpressionExtensionsTests
{
    [Fact]
    public void SplitMemberAccessChain_RecursiveDataStructure_ReturnsExpressionForEachMemberAccess()
    {
        Expression<Func<MembershipGroup, MembershipGroup>>
            expression = mg => mg.Parent!.Parent!.Parent!.Parent!.Parent!.Parent!;

        var result = expression.SplitMemberAccessChain().ToList();

        const int numberOfParentAccesses = 6;
        Expression<Func<MembershipGroup, MembershipGroup>> expected = x => x.Parent!;
        result.Count.ShouldBe(numberOfParentAccesses, "we should have an expression for each member access");
        foreach (var expressionInChain in result)
        {
            expressionInChain.ShouldBeEquivalentTo(
                expected,
                "each returned item should be a single-level member access to its parent");
        }
    }

    [Fact]
    public void SplitMemberAccessChain_TypicalDataStructure_ReturnsExpressionForEachMemberAccess()
    {
        Expression<Func<Booking, Region>> expression = b => b.Facility.Room!.Region;

        var result = expression.SplitMemberAccessChain().ToArray();

        const int numberOfMemberAccesses = 3;
        Expression<Func<Booking, Facility>> expectedFirst = x => x.Facility;
        Expression<Func<Facility, Room>> expectedSecond = x => x.Room!;
        Expression<Func<Room, Region>> expectedThird = x => x.Region;
        result.ShouldSatisfyAllConditions(
            r => r.Length.ShouldBe(numberOfMemberAccesses, "we should have an expression for each member access"),
            r => r[0].ShouldBeEquivalentTo(expectedFirst, $"the first item returned should be the {nameof(Booking)} accessing its {nameof(Booking.Facility)}"),
            r => r[1].ShouldBeEquivalentTo(expectedSecond, $"the second item returned should be the {nameof(Facility)} accessing its {nameof(Facility.Room)}"),
            r => r[2].ShouldBeEquivalentTo(expectedThird, $"the third item returned should be the {nameof(Room)} accessing its {nameof(Room.Region)}"));
    }

    [Fact]
    public void SplitMemberAccessChain_ChainContainsExplicitCast_CastingIsPreserved()
    {
        Expression<Func<CompanyAssetValue, Employee>> expression = caa => ((EmployeeAsset)caa.CompanyAsset.Asset).Employee;

        var result = expression.SplitMemberAccessChain().ToArray();

        const int numberOfMemberAccesses = 3;
        Expression<Func<CompanyAssetValue, CompanyAsset>> expectedFirst = x => x.CompanyAsset;
        Expression<Func<CompanyAsset, EmployeeAsset>> expectedSecond = x => (EmployeeAsset)x.Asset;
        Expression<Func<EmployeeAsset, Employee>> expectedThird = x => x.Employee;
        result.ShouldSatisfyAllConditions(
            r => r.Length.ShouldBe(numberOfMemberAccesses, "we should have an expression for each member access"),
            r => r[0].ShouldBeEquivalentTo(expectedFirst, $"the first item returned should be the {nameof(CompanyAssetValue)} accessing its {nameof(CompanyAssetValue.CompanyAsset)}"),
            r => r[1].ShouldBeEquivalentTo(expectedSecond, $"the second item returned should be the {nameof(CompanyAsset)} accessing its {nameof(CompanyAsset.Asset)}, casted to {nameof(EmployeeAsset)}"),
            r => r[2].ShouldBeEquivalentTo(expectedThird, $"the second item returned should be the {nameof(EmployeeAsset)} accessing its {nameof(EmployeeAsset.Employee)}"));
    }

    /// <summary>
    /// This failed after switching from FluentAssertions to Shouldly. FluentAssertions' default 'BeEquivalentTo' didn't compare the expression tree
    /// deeply enough to notice that JoinMemberAccessChain was dropping the member access before a cast in the middle of the chain.
    /// JoinMemberAccessChain was fixed so the full chain is kept.
    /// </summary>
    [Fact]
    public void JoinMemberAccessChain_MiddleExpressionContainsExplicitCast_JoinedExpressionPreservesTheCast()
    {
        var result = JoinCompanyAssetValueToEmployeeViaCast();
        Expression<Func<CompanyAssetValue, Employee>> expected = x => ((EmployeeAsset)x.CompanyAsset.Asset).Employee;
        result.ShouldBeEquivalentTo(expected, "we should join up the lambdas to form a single expression containing the cast");
    }

    /// <summary>
    /// This essentially covers the same outcome as <see cref="JoinMemberAccessChain_MiddleExpressionContainsExplicitCast_JoinedExpressionPreservesTheCast"/>
    /// but it's easier to follow, as the logic is not buried in the 'ShouldBeEquivalentTo' comparison, and it checks the expression tree structure directly.
    /// </summary>
    [Fact]
    public void JoinMemberAccessChain_MiddleExpressionContainsExplicitCast_JoinedExpressionKeepsEveryMemberAccess()
    {
        var result = JoinCompanyAssetValueToEmployeeViaCast();

        // Expected: x => ((EmployeeAsset)x.CompanyAsset.Asset).Employee, checked from the outside in.
        var employeeAccess = result.Body.ShouldBeAssignableTo<MemberExpression>();
        employeeAccess.Member.Name.ShouldBe(nameof(EmployeeAsset.Employee));

        var cast = employeeAccess.Expression.ShouldBeOfType<UnaryExpression>();
        cast.NodeType.ShouldBe(ExpressionType.Convert);
        cast.Type.ShouldBe(typeof(EmployeeAsset));

        var assetAccess = cast.Operand.ShouldBeAssignableTo<MemberExpression>();
        assetAccess.Member.Name.ShouldBe(nameof(CompanyAsset.Asset));

        var companyAssetAccess = assetAccess.Expression.ShouldBeAssignableTo<MemberExpression>(
            $"the {nameof(CompanyAssetValue.CompanyAsset)} access before the cast should not be lost");

        companyAssetAccess!.Member.Name.ShouldBe(nameof(CompanyAssetValue.CompanyAsset));
        companyAssetAccess.Expression!.ShouldBeSameAs(
            result.Parameters[0],
            "the chain should start from the joined lambda's own parameter");
    }

    /// <summary>
    /// This provides a realistic test of the joined expression, ensuring it can be compiled and invoked to navigate through the casted member access
    /// chain, and retrieve the expected employee.
    /// </summary>
    [Fact]
    public void JoinMemberAccessChain_MiddleExpressionContainsExplicitCast_JoinedExpressionCanBeCompiledAndInvoked()
    {
        var employee = new Employee { FirstName = "John", LastName = "Smith" };
        var companyAssetValue = new CompanyAssetValue
        {
            CompanyAsset = new CompanyAsset { Asset = new EmployeeAsset("Laptop") { Employee = employee } }
        };

        var result = JoinCompanyAssetValueToEmployeeViaCast();
        var compiled = result.Compile();
        Employee? invoked = compiled.DynamicInvoke(companyAssetValue) as Employee;

        invoked.ShouldBeSameAs(
            employee,
            "the joined expression should navigate from the value, through the cast asset, to its employee");
    }

    [Fact]
    public void JoinMemberAccessChain_FirstExpressionContainsExplicitCast_JoinedExpressionPreservesTheCast()
    {
        Expression<Func<CompanyAsset, EmployeeAsset>> first = x => (EmployeeAsset)x.Asset;
        Expression<Func<EmployeeAsset, Employee>> second = x => x.Employee;

        IEnumerable<LambdaExpression> target = [first, second];

        var result = target.JoinMemberAccessChain();

        Expression<Func<CompanyAsset, Employee>> expected = x => ((EmployeeAsset)x.Asset).Employee;

        result.ShouldBeEquivalentTo(expected, "we should join up the lambdas to form a single expression containing the cast");
    }

    [Fact]
    public void JoinMemberAccessChain_TypicalDataStructure_ReturnsSingleExpressionCombiningEachLambda()
    {
        Expression<Func<Booking, Facility>> first = b => b.Facility;
        Expression<Func<Facility, Room>> second = f => f.Room!;
        Expression<Func<Room, Region>> third = r => r.Region;

        IEnumerable<LambdaExpression> target = [first, second, third];

        var result = target.JoinMemberAccessChain();

        Expression<Func<Booking, Region>> expected = b => b.Facility.Room!.Region;
        result.ShouldBeEquivalentTo(expected, "we should join up the lambdas to form a single expression");
    }

    [Fact]
    public void JoinMemberAccessChain_LambdaParameterDoesNotMatchPredecessorBody_ExceptionThrown()
    {
        Expression<Func<Booking, Facility>> first = b => b.Facility;
        Expression<Func<Booking, Member>> incompatibleLambda = b => b.Member;
        Expression<Func<Room, Region>> third = r => r.Region;

        IEnumerable<LambdaExpression> target = [first, incompatibleLambda, third];

        var act = () => target.JoinMemberAccessChain();

        // This isn't a handled exception as it's not a mistake a developer can make, but an internal error.
        // I just want to assert that the code throws an exception in this scenario.
        act.ShouldThrow<ArgumentException>();
    }

    private static LambdaExpression JoinCompanyAssetValueToEmployeeViaCast()
    {
        Expression<Func<CompanyAssetValue, CompanyAsset>> first = x => x.CompanyAsset;
        Expression<Func<CompanyAsset, EmployeeAsset>> second = x => (EmployeeAsset)x.Asset;
        Expression<Func<EmployeeAsset, Employee>> third = x => x.Employee;

        IEnumerable<LambdaExpression> target = [first, second, third];

        return target.JoinMemberAccessChain();
    }
}