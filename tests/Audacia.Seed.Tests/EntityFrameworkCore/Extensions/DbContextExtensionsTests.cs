using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Entities.Enums;
using Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using Audacia.Seed.Tests.TestHelpers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Xunit;

namespace Audacia.Seed.Tests.EntityFrameworkCore.Extensions;

[Collection(CollectionNames.TestDatabaseContextCollection)]
public sealed class DbContextExtensionsTests : IDisposable
{
    private readonly TestDatabaseContext _context;

    public DbContextExtensionsTests()
    {
        _context = TestDatabaseContextBuilder.CreateContext();

        _transaction = _context.Database.BeginTransaction();
    }

    [Fact]
    public async Task SeedMany_AmountToCreateSpecified_EntitiesAdded()
    {
        const int amountToCreate = 100;
        var seedConfiguration = new FacilitySeed();

        _context.SeedMany(amountToCreate, seedConfiguration);

        var count = await _context.Set<Facility>().CountAsync();
        count.Should().Be(amountToCreate, $"{nameof(amountToCreate)} should create the specified number of entities.");
    }

    [Fact]
    public async Task SeedMany_AmountToCreateIsMoreThanOne_AllChildrenHaveTheSameParent()
    {
        const int amountToCreate = 10;

        var seedConfiguration = new BookingSeed();
        _context.SeedMany(amountToCreate, seedConfiguration);

        const int expectedCount = 1;
        var savedEntities = await _context.Set<Booking>().ToListAsync();
        savedEntities.DistinctBy(b => b.MemberId).Should()
            .HaveCount(expectedCount, "child entities should share parents by default");
    }

    [Fact]
    public async Task Seed_PassingInMultipleEntities_AllEntitiesSeeded()
    {
        _context.Seed(new BookingSeed(), new BookingSeed(), new BookingSeed());

        const int expectedCount = 3;
        var savedEntities = await _context.Set<Booking>().ToListAsync();
        savedEntities
            .Should()
            .HaveCount(expectedCount, "the seed params method should seed each booking");
    }

    [Fact]
    public async Task Seed_PassingSingleEntitiesMultipleTimes_AllEntitiesSeeded()
    {
        _context.Seed(new FacilitySeed());
        _context.Seed(new FacilitySeed());
        _context.Seed(new FacilitySeed());

        const int expectedCount = 3;
        var savedEntities = await _context.Set<Facility>().ToListAsync();
        savedEntities
            .Should()
            .HaveCount(expectedCount, "the seed method should seed an entity every time");
    }

    [Fact]
    public async Task Seed_PassingSingleEntitiesWhichMustFindExisting_AllEntitiesSeeded()
    {
        _context.Seed(new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());

        const int expectedCount = 1;
        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync();
        savedEntities
            .Should()
            .HaveCount(expectedCount, "the seed method should not seed duplicate if we must find existing");
    }

    [Fact]
    public async Task SeedMany_PassingEntitiesWhichMustFindExisting_AllEntitiesSeeded()
    {
        var expectedCount = Enum.GetValues<FacilityType>().Length;
        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync();
        savedEntities
            .Should()
            .HaveCount(expectedCount, "we should be able to seed multiple entities with must find existing");
    }

    [Fact]
    public async Task SeedMany_PassingEntitiesWhichMustFindExistingMultipleTimes_DuplicatesNotSeeded()
    {
        var expectedCount = Enum.GetValues<FacilityType>().Length;

        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());
        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync();
        using (new AssertionScope())
        {
            savedEntities.Should().HaveCount(expectedCount, "we should not seed duplicates");
            savedEntities.Select(se => se.Type).Distinct().Should()
                .HaveCount(expectedCount, "each facility type should have a unique type");
        }
    }

    [Fact]
    public async Task Seed_SpecifyingCustomisedEntityWithHardcodedIds_IsSeededSuccessfully()
    {
        _context.Seed(new FacilityTypeEntitySeed()
            .With(f => f.Type, FacilityType.TennisCourt)
            .Without(f => f.Description));

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync();

        using (new AssertionScope())
        {
            savedEntities.Should().HaveCount(1, "we should only seed one entity");
            savedEntities.First().Type.Should().Be(FacilityType.TennisCourt, "the seeded entity should have the correct type");
            savedEntities.First().Description.Should().BeNull("the seeded entity should not have a description");
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction.Dispose();
    }
}