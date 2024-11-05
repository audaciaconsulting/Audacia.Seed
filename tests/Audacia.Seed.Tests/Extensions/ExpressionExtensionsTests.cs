using System.Linq.Expressions;
using Audacia.Core.Extensions;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Xunit;
using Audacia.Seed.Extensions;
using FluentAssertions;
using FluentAssertions.Execution;

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
        using (new AssertionScope())
        {
            result.Should().HaveCount(numberOfParentAccesses, "we should have an expression for each member access");
            result.Should().AllBeEquivalentTo(
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
        using (new AssertionScope())
        {
            result.Should().HaveCount(numberOfMemberAccesses, "we should have an expression for each member access");
            result[0].Should().BeEquivalentTo(expectedFirst, $"the first item returned should be the {nameof(Booking)} accessing its {nameof(Booking.Facility)}");
            result[1].Should().BeEquivalentTo(expectedSecond, $"the second item returned should be the {nameof(Facility)} accessing its {nameof(Facility.Room)}");
            result[2].Should().BeEquivalentTo(expectedThird, $"the third item returned should be the {nameof(Room)} accessing its {nameof(Room.Region)}");
        }
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
        using (new AssertionScope())
        {
            result.Should().HaveCount(numberOfMemberAccesses, "we should have an expression for each member access");
            result[0].Should().BeEquivalentTo(expectedFirst, $"the first item returned should be the {nameof(CompanyAssetValue)} accessing its {nameof(CompanyAssetValue.CompanyAsset)}");
            result[1].Should().BeEquivalentTo(expectedSecond, $"the second item returned should be the {nameof(CompanyAsset)} accessing its {nameof(CompanyAsset.Asset)}, casted to {nameof(EmployeeAsset)}");
            result[2].Should().BeEquivalentTo(expectedThird, $"the second item returned should be the {nameof(EmployeeAsset)} accessing its {nameof(EmployeeAsset.Employee)}");
        }
    }

    [Fact]
    public void JoinMemberAccessChain_MiddleExpressionContainsExplicitCast_JoinedExpressionPreservesTheCast()
    {
        Expression<Func<CompanyAssetValue, CompanyAsset>> first = x => x.CompanyAsset;
        Expression<Func<CompanyAsset, EmployeeAsset>> second = x => (EmployeeAsset)x.Asset;
        Expression<Func<EmployeeAsset, Employee>> third = x => x.Employee;

        IEnumerable<LambdaExpression> target = [first, second, third];

        var result = target.JoinMemberAccessChain();

        Expression<Func<CompanyAssetValue, Employee>> expected = x => ((EmployeeAsset)x.CompanyAsset.Asset).Employee;

        result.Should().BeEquivalentTo(expected, "we should join up the lambdas to form a single expression containing the cast");
    }

    [Fact]
    public void JoinMemberAccessChain_FirstExpressionContainsExplicitCast_JoinedExpressionPreservesTheCast()
    {
        Expression<Func<CompanyAsset, EmployeeAsset>> first = x => (EmployeeAsset)x.Asset;
        Expression<Func<EmployeeAsset, Employee>> second = x => x.Employee;

        IEnumerable<LambdaExpression> target = [first, second];

        var result = target.JoinMemberAccessChain();

        Expression<Func<CompanyAsset, Employee>> expected = x => ((EmployeeAsset)x.Asset).Employee;

        result.Should().BeEquivalentTo(expected, "we should join up the lambdas to form a single expression containing the cast");
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
        result.Should().BeEquivalentTo(expected, "we should join up the lambdas to form a single expression");
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
        act.Should().Throw<ArgumentException>();
    }
}