using Audacia.Seed.Customisation;
using Audacia.Seed.InMemory;
using Audacia.Seed.Options;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using Shouldly;
using Xunit;

namespace Audacia.Seed.Tests;

public class EntitySeedBuildManyTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(10)]
    public void BuildMany_AmountProvided_ReturnsThatManyDistinctEntities(int amountToCreate)
    {
        var seed = new CouponSeed { Repository = new InMemorySeedableRepository() };

        var coupons = seed.BuildMany(amountToCreate);

        coupons.Count.ShouldBe(amountToCreate);
        coupons.Distinct().Count().ShouldBe(amountToCreate, "each entity should be a separate instance");
    }

    [Fact]
    public void BuildMany_ResultNotEnumerated_AllEntitiesAreAddedToTheRepository()
    {
        const int amountToCreate = 3;
        var repository = new InMemorySeedableRepository();
        var seed = new CouponSeed { Repository = repository };

        _ = seed.BuildMany(amountToCreate);

        repository.DbSet<Coupon>().Count().ShouldBe(
            amountToCreate,
            "entities should be added when BuildMany is called, without the caller having to enumerate the result");
    }

    [Theory]
    [InlineData(1, SeedingInsertionBehaviour.TryFindNew)]
    [InlineData(3, SeedingInsertionBehaviour.AddNew)]
    public void BuildMany_ResultNotEnumerated_OptionsAreUpdated(int amountToCreate, SeedingInsertionBehaviour expectedBehaviour)
    {
        var seed = new CouponSeed { Repository = new InMemorySeedableRepository() };

        _ = seed.BuildMany(amountToCreate);

        seed.Options.ShouldSatisfyAllConditions(
            o => o.AmountToCreate.ShouldBe(amountToCreate),
            o => o.InsertionBehavior.ShouldBe(expectedBehaviour));
    }

    [Fact]
    public void BuildMany_IndexBasedCustomisation_EntitiesAreReturnedInIndexOrder()
    {
        var seed = new CouponSeed { Repository = new InMemorySeedableRepository() }
            .With(c => c.Discount, index => index * 10m);

        var coupons = seed.BuildMany(5);

        coupons.Select(c => c.Discount).ShouldBe([0m, 10m, 20m, 30m, 40m]);
    }

    [Fact]
    public void BuildMany_PreviousBasedCustomisation_EachEntityIsBuiltFromTheOneBeforeIt()
    {
        var seed = new CouponSeed { Repository = new InMemorySeedableRepository() }
            .With(c => c.Discount, (_, previous) => (previous?.Discount * 2) ?? 0.1m);

        var coupons = seed.BuildMany(5);

        coupons.Select(c => c.Discount).ShouldBe([0.1m, 0.2m, 0.4m, 0.8m, 1.6m]);
    }

    [Fact]
    public void BuildMany_GetDefaultUsesPrevious_ReceivesTheEntityReturnedAtThePreviousIndex()
    {
        var seed = new PreviousTrackingCouponSeed { Repository = new InMemorySeedableRepository() };

        var coupons = seed.BuildMany(4);

        seed.PreviousByIndex.Keys.ShouldBe([0, 1, 2, 3]);
        seed.ShouldSatisfyAllConditions(
            s => s.PreviousByIndex[0].ShouldBeNull("the first entity has no previous entity"),
            s => s.PreviousByIndex[1].ShouldBeSameAs(coupons[0]),
            s => s.PreviousByIndex[2].ShouldBeSameAs(coupons[1]),
            s => s.PreviousByIndex[3].ShouldBeSameAs(coupons[2]));
    }

    [Fact]
    public void BuildMany_CalledTwiceOnTheSameSeed_PreviousStartsAgainForEachCall()
    {
        var seed = new CouponSeed { Repository = new InMemorySeedableRepository() }
            .With(c => c.Discount, (_, previous) => (previous?.Discount * 2) ?? 0.1m);

        var first = seed.BuildMany(3);
        var second = seed.BuildMany(3);

        first.Select(c => c.Discount).ShouldBe([0.1m, 0.2m, 0.4m]);
        second.Select(c => c.Discount).ShouldBe(
            [0.1m, 0.2m, 0.4m],
            "the second call should not carry on from the last entity of the first call");
        first.Intersect(second).ShouldBeEmpty("each call should build new entities");
    }

    private sealed class PreviousTrackingCouponSeed : EntitySeed<Coupon>
    {
        public Dictionary<int, Coupon?> PreviousByIndex { get; } = [];

        protected override Coupon GetDefault(int index, Coupon? previous)
        {
            PreviousByIndex[index] = previous;
            return new Coupon(Guid.NewGuid().ToString());
        }
    }
}
