using Audacia.Seed.Customisation;
using Audacia.Seed.InMemory;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using FluentAssertions;
using Xunit;

namespace Audacia.Seed.Tests.InMemory;

public class EntityBuilderTests
{
    [Fact]
    public void Build_NoSeedProvided_BuildsRequiredRelationshipsCorrectly()
    {
        var booking = new EntityBuilder()
            .Build<Booking>();

        booking.Member.Should().NotBeNull();
        booking.Facility.Should().NotBeNull();
        booking.Facility.Owner.Should().NotBeNull();
        booking.Facility.Manager.Should().NotBeNull();
    }

    [Fact]
    public void Build_SeedProvided_BuildsRequiredRelationshipsCorrectly()
    {
        var booking = new EntityBuilder()
            .Build(new BookingSeed());

        booking.Member.Should().NotBeNull();
        booking.Facility.Should().NotBeNull();
        booking.Facility.Owner.Should().NotBeNull();
        booking.Facility.Manager.Should().NotBeNull();
    }

    [Fact]
    public void Build_TwoSeedsOfSameTypeProvided_EntitiesShareParents()
    {
        var (firstBooking, secondBooking) = new EntityBuilder()
            .Build(new BookingSeed(), new BookingSeed());

        firstBooking.Should().NotBe(secondBooking);
        firstBooking.Facility.Should().Be(secondBooking.Facility);
        firstBooking.Member.Should().Be(secondBooking.Member);
    }

    [Fact]
    public void Build_BuildInSeparateActions_EntitiesShareParents()
    {
        var builder = new EntityBuilder();
        var firstBooking = builder.Build<Booking>();
        var secondBooking = builder.Build<Booking>();

        firstBooking.Should().NotBe(secondBooking);
        firstBooking.Facility.Should().Be(secondBooking.Facility);
        firstBooking.Member.Should().Be(secondBooking.Member);
    }

    [Fact]
    public void Build_SeedUsesWithExisting_FindsEntityCorrectly()
    {
        var builder = new EntityBuilder();
        var facilityName = Guid.NewGuid().ToString();
        var facility = builder.Build(new FacilitySeed().With(f => f.Name, facilityName));
        var secondBooking = builder.Build(new BookingSeed()
            .WithExisting(b => b.Facility, f => f.Name == facilityName));

        secondBooking.Facility.Name.Should().Be(facilityName);
        secondBooking.Facility.Should().Be(facility);
    }

    [Fact]
    public void BuildMany_EntityHasRequiredParents_EntitiesShareParents()
    {
        const int amountToCreate = 5;
        var bookings = new EntityBuilder().BuildMany(amountToCreate, new BookingSeed()).ToList();

        bookings.Distinct().Should().HaveCount(amountToCreate);
        bookings.Select(b => b.Facility).Distinct().Should().HaveCount(1);
    }

    [Fact]
    public void Build_UsesDifferentEntityBuilder_CreatesTwoDifferentEntities()
    {
        var firstCoupon = new EntityBuilder().Build(new CouponSeed());
        var secondCoupon = new EntityBuilder().Build(new CouponSeed());

        firstCoupon.Should().NotBe(secondCoupon);
    }
}