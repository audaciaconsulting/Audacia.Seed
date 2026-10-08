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
        var (firstBooking, secondBooking) = new EntityBuilder()
            .Build(new BookingSeed(), new BookingSeed());

        firstBooking.ShouldNotBe(secondBooking);
        firstBooking.Facility.ShouldBe(secondBooking.Facility);
        firstBooking.Member.ShouldBe(secondBooking.Member);
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
    }

    [Fact]
    public void BuildMany_EntityHasRequiredParents_EntitiesShareParents()
    {
        const int amountToCreate = 5;
        var bookings = new EntityBuilder().BuildMany(amountToCreate, new BookingSeed()).ToList();

        bookings.Distinct().Count().ShouldBe(amountToCreate);
        bookings.Select(b => b.Facility).Distinct().Count().ShouldBe(1);
    }

    [Fact]
    public void Build_UsesDifferentEntityBuilder_CreatesTwoDifferentEntities()
    {
        var firstCoupon = new EntityBuilder().Build(new CouponSeed());
        var secondCoupon = new EntityBuilder().Build(new CouponSeed());

        firstCoupon.ShouldNotBe(secondCoupon);
    }
}