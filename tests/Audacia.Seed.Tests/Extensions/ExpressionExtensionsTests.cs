using System.Linq.Expressions;
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
            expression = x => x.Parent!.Parent!.Parent!.Parent!.Parent!.Parent!;

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
        Expression<Func<Booking, Region>> expression = x => x.Facility.Room!.Region;

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
    public void JoinMemberAccessChain_TypicalDataStructure_ReturnsSingleExpressionCombiningEachLambda()
    {
        Expression<Func<Booking, Facility>> expectedFirst = x => x.Facility;
        Expression<Func<Facility, Room>> expectedSecond = x => x.Room!;
        Expression<Func<Room, Region>> expectedThird = x => x.Region;

        IEnumerable<LambdaExpression> target = [expectedFirst, expectedSecond, expectedThird];

        var result = target.JoinMemberAccessChain();

        Expression<Func<Booking, Region>> expected = x => x.Facility.Room!.Region;
        result.Should().BeEquivalentTo(expected, "we should join up the lambdas to form a single expression");
    }

    [Fact]
    public void JoinMemberAccessChain_LambdaParameterDoesNotMatchPredecessorBody_ExceptionThrown()
    {
        Expression<Func<Booking, Facility>> expectedFirst = x => x.Facility;
        Expression<Func<Booking, Member>> incompatibleLambda = x => x.Member;
        Expression<Func<Room, Region>> expectedThird = x => x.Region;

        IEnumerable<LambdaExpression> target = [expectedFirst, incompatibleLambda, expectedThird];

        var act = () => target.JoinMemberAccessChain();

        // Not a handled exception as it's not a mistake a developer can make, but an internal error. I just want to assert that can't handle it.
        act.Should().Throw<ArgumentException>();
    }
}