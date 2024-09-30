using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.EntityFrameworkCore.Repositories;
using Audacia.Seed.Exceptions;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using Audacia.Seed.Tests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Audacia.Seed.Tests;

[Collection(CollectionNames.TestDatabaseContextCollection)]
public sealed class EntitySeedTests : IDisposable
{
    private readonly TestDatabaseContext _context;
    private readonly IDbContextTransaction _transaction;

    public EntitySeedTests()
    {
        _context = TestDatabaseContextBuilder.CreateContext();

        _transaction = _context.Database.BeginTransaction();
    }

    [Fact]
    public void SpecifySeedConfiguration_EntityIsAddedToTheDatabase()
    {
        var seedConfiguration = new BookingSeed();

        var booking = _context.Seed(seedConfiguration);

        var savedEntity = _context.Set<Booking>().Find(booking.Id);
        savedEntity.Should().NotBeNull("the entity should have been saved to the database.");
    }

    [Fact]
    public void Prerequisites_EntityHasParentsOfTheSameType_PrerequisitesContainBothParent()
    {
        var seed = new EntitySeed<Facility>();
        seed.Repository = new EntityFrameworkCoreSeedableRepository(_context);

        var prerequisites = seed.Prerequisites().ToList();

        using (new AssertionScope())
        {
            prerequisites.Should().Contain(
                p => p.PropertyInfo.Name == nameof(Facility.Manager) && p.EntityType == typeof(Employee),
                $"one of the prerequisites should be for the {nameof(Facility.Manager)}");
            prerequisites.Should().Contain(
                p => p.PropertyInfo.Name == nameof(Facility.Owner) && p.EntityType == typeof(Employee),
                $"one of the prerequisites should be for the {nameof(Facility.Owner)}");
        }
    }

    [Fact]
    public void EntityDoesNotHaveSeedClass_CanBeSeeded()
    {
        var seed = new EntitySeed<Region>();

        var seededEntity = _context.Seed(seed);

        seededEntity.Id.Should().BeGreaterThan(0, "we should be able to seed entities without a dedicated class");
    }

    [Fact]
    public async Task EntityHasRequiredParent_CanBeSeededWithoutDedicatedClass()
    {
        var seed = new EntitySeed<Booking>()
            .With(b => b.Facility.Name, "Test facility")
            .With(b => b.Member.FirstName, "Test first name")
            .With(b => b.Member.LastName, "Test last name");

        var seededEntity = _context.Seed(seed);

        var member = await _context.Set<Member>().SingleOrDefaultAsync(m => m.Id == seededEntity.MemberId);
        member.Should().NotBeNull("we should be able to seed required parents without the need for a dedicated class");
    }

    [Fact]
    public void EntityDoesNotHaveParameterlessConstructor_CanBeSeededWithoutDedicatedClass()
    {
        var seed = new EntitySeed<Coupon>();

        var seededEntity = _context.Seed(seed);

        seededEntity.Name.Should()
            .NotBeNull($"we should use the constructor of {nameof(Coupon)} that populates the {nameof(Coupon.Name)}");
    }

    [Fact]
    public void EntityHasOptionalParent_IsNotSeeded()
    {
        var seed = new FacilitySeed();

        var seededEntity = _context.Seed(seed);

        seededEntity.RoomId.Should()
            .BeNull("optional parents should not be seeded");
    }

    [Fact]
    public void WithPrerequisite_GetterDoesNotAccessMemberOnEntity_ExceptionThrown()
    {
        var act = () => new EntitySeed<Booking>()
            .WithNew(_ => new Region());

        act.Should().ThrowExactly<DataSeedingException>("we should throw an exception if the getter does not access a property on the entity");
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction.Dispose();
    }
}