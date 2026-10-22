using System.Reflection;
using Audacia.Seed.Customisation;
using Audacia.Seed.InMemory;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using Shouldly;
using Xunit;

namespace Audacia.Seed.Tests.InMemory;

public class EntityBuilderTests
{
    [Fact]
    public void Build_NoSeedProvided_BuildsRequiredRelationshipsCorrectly()
    {
        var booking = new EntityBuilder()
            .Build<Booking>();

        booking.Member.ShouldNotBeNull();
        booking.Facility.ShouldNotBeNull();
        booking.Facility.Owner.ShouldNotBeNull();
        booking.Facility.Manager.ShouldNotBeNull();
    }

    [Fact]
    public void Build_NoSeedProvided_DoesNotBuildOptionalRelationships()
    {
        var booking = new EntityBuilder()
            .Build<Booking>();

        booking.Coupon.ShouldBeNull();
    }

    [Fact]
    public void Build_SeedProvided_BuildsRequiredRelationshipsCorrectly()
    {
        var booking = new EntityBuilder()
            .Build(new BookingSeed());

        booking.Member.ShouldNotBeNull();
        booking.Facility.ShouldNotBeNull();
        booking.Facility.Owner.ShouldNotBeNull();
        booking.Facility.Manager.ShouldNotBeNull();
    }

    [Fact]
    public void Build_TwoSeedsOfSameTypeProvided_EntitiesShareParents()
    {
        var builder = new EntityBuilder();
        var (firstBooking, secondBooking) = builder
            .Build(new BookingSeed(), new BookingSeed());

        firstBooking.ShouldNotBe(secondBooking);
        firstBooking.Facility.ShouldBe(secondBooking.Facility);
        firstBooking.Member.ShouldBe(secondBooking.Member);
        StoredEntities<Booking>(builder).ShouldBe([firstBooking, secondBooking]);
        StoredEntities<Facility>(builder).ShouldBe([firstBooking.Facility]);
        StoredEntities<Member>(builder).ShouldBe([firstBooking.Member]);
    }

    [Fact]
    public void Build_BuildInSeparateActions_EntitiesShareParents()
    {
        var builder = new EntityBuilder();
        var firstBooking = builder.Build<Booking>();
        var secondBooking = builder.Build<Booking>();

        firstBooking.ShouldNotBe(secondBooking);
        firstBooking.Facility.ShouldBe(secondBooking.Facility);
        firstBooking.Member.ShouldBe(secondBooking.Member);
        StoredEntities<Booking>(builder).ShouldBe([firstBooking, secondBooking]);
        StoredEntities<Facility>(builder).ShouldBe([firstBooking.Facility]);
        StoredEntities<Member>(builder).ShouldBe([firstBooking.Member]);
    }

    [Fact]
    public void Build_SeedUsesWithExisting_FindsEntityCorrectly()
    {
        var builder = new EntityBuilder();
        var facilityName = Guid.NewGuid().ToString();
        var facility = builder.Build(new FacilitySeed().With(f => f.Name, facilityName));
        var booking = builder.Build(new BookingSeed()
            .WithExisting(b => b.Facility, f => f.Name == facilityName));

        booking.Facility.Name.ShouldBe(facilityName);
        booking.Facility.ShouldBe(facility);
        StoredEntities<Facility>(builder).ShouldBe([facility], "the existing facility should be reused, not stored again");
    }

    [Fact]
    public void BuildMany_EntityHasRequiredParents_EntitiesShareParents()
    {
        const int amountToCreate = 5;
        var builder = new EntityBuilder();
        var bookings = builder.BuildMany(amountToCreate, new BookingSeed()).ToList();

        bookings.Distinct().Count().ShouldBe(amountToCreate);
        bookings.Select(b => b.Facility).Distinct().Count().ShouldBe(1);
        StoredEntities<Booking>(builder).ShouldBe(bookings);
    }

    [Fact]
    public void Build_UsesDifferentEntityBuilder_CreatesTwoDifferentEntities()
    {
        var firstCoupon = new EntityBuilder().Build(new CouponSeed());
        var secondCoupon = new EntityBuilder().Build(new CouponSeed());

        firstCoupon.ShouldNotBe(secondCoupon);
    }

    [Fact]
    public void Build_SeedProvided_EntityIsStoredOnce()
    {
        var builder = new EntityBuilder();

        var coupon = builder.Build(new CouponSeed());

        StoredEntities<Coupon>(builder).ShouldBe([coupon]);
    }

    [Fact]
    public void Build_TwoSeedsProvided_EachEntityIsStoredOnce()
    {
        var builder = new EntityBuilder();

        var (firstCoupon, secondCoupon) = builder.Build(new CouponSeed(), new CouponSeed());

        StoredEntities<Coupon>(builder).ShouldBe([firstCoupon, secondCoupon]);
    }

    [Fact]
    public void BuildMany_SeedProvided_EachEntityIsStoredOnce()
    {
        var builder = new EntityBuilder();

        var coupons = builder.BuildMany(3, new CouponSeed()).ToList();

        StoredEntities<Coupon>(builder).ShouldBe(coupons);
    }

    [Fact]
    public void BuildMany_NoSeedProvided_EachEntityIsStoredOnce()
    {
        var builder = new EntityBuilder();

        var coupons = builder.BuildMany<Coupon>(3).ToList();

        StoredEntities<Coupon>(builder).ShouldBe(coupons);
    }

    [Fact]
    public void BuildMany_EntityHasRequiredParents_SharedParentIsStoredOnce()
    {
        var builder = new EntityBuilder();

        var bookings = builder.BuildMany(3, new BookingSeed()).ToList();

        StoredEntities<Facility>(builder).ShouldBe([bookings[0].Facility]);
    }

    /// <summary>
    /// The in-memory store isn't exposed publicly, and lookups only take the first match, so read it directly to detect duplicates.
    /// </summary>
    private static List<TEntity> StoredEntities<TEntity>(EntityBuilder builder)
        where TEntity : class
    {
        var repository = (InMemorySeedableRepository)typeof(EntityBuilder)
            .GetField("_repo", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(builder)!;

        return [.. repository.DbSet<TEntity>()];
    }
}