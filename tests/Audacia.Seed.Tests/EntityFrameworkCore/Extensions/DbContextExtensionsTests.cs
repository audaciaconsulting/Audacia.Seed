using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Entities.Enums;
using Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;
using Audacia.Seed.Tests.ExampleProject.Seeds;
using Audacia.Seed.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Shouldly;
using Xunit;

namespace Audacia.Seed.Tests.EntityFrameworkCore.Extensions;

[Collection(CollectionNames.TestDatabaseContextCollection)]
public sealed class DbContextExtensionsTests : IDisposable
{
    private readonly TestDatabaseContext _context;
    private readonly IDbContextTransaction _transaction;

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

        var count = await _context.Set<Facility>().CountAsync(TestContext.Current.CancellationToken);

        count.ShouldBe(amountToCreate, $"{nameof(amountToCreate)} should create the specified number of entities.");
    }

    [Fact]
    public async Task SeedMany_AmountToCreateIsMoreThanOne_AllChildrenHaveTheSameParent()
    {
        const int amountToCreate = 10;

        var seedConfiguration = new BookingSeed();
        _context.SeedMany(amountToCreate, seedConfiguration);

        const int expectedCount = 1;
        var savedEntities = await _context.Set<Booking>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities.DistinctBy(b => b.MemberId).Count().ShouldBe(expectedCount, "child entities should share parents by default");
    }

    [Fact]
    public async Task Seed_PassingInMultipleEntities_AllEntitiesSeeded()
    {
        _context.Seed(new BookingSeed(), new BookingSeed(), new BookingSeed());

        const int expectedCount = 3;
        var savedEntities = await _context.Set<Booking>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities
            .Count.ShouldBe(expectedCount, "the seed params method should seed each booking");
    }

    [Fact]
    public async Task Seed_PassingSingleEntitiesMultipleTimes_AllEntitiesSeeded()
    {
        _context.Seed(new FacilitySeed());
        _context.Seed(new FacilitySeed());
        _context.Seed(new FacilitySeed());

        const int expectedCount = 3;
        var savedEntities = await _context.Set<Facility>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities
            .Count.ShouldBe(expectedCount, "the seed method should seed an entity every time");
    }

    [Fact]
    public async Task Seed_PassingSingleEntitiesWhichMustFindExisting_AllEntitiesSeeded()
    {
        _context.Seed(new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());

        const int expectedCount = 1;
        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities
            .Count.ShouldBe(expectedCount, "the seed method should not seed duplicate if we must find existing");
    }

    [Fact]
    public async Task SeedMany_PassingEntitiesWhichMustFindExisting_AllEntitiesSeeded()
    {
        var expectedCount = Enum.GetValues<FacilityType>().Length;
        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities
            .Count.ShouldBe(expectedCount, "we should be able to seed multiple entities with must find existing");
    }

    [Fact]
    public async Task SeedMany_PassingEntitiesWhichMustFindExistingMultipleTimes_DuplicatesNotSeeded()
    {
        var expectedCount = Enum.GetValues<FacilityType>().Length;

        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());
        _context.SeedMany(expectedCount, new FacilityTypeEntitySeed());
        _context.Seed(new FacilityTypeEntitySeed());

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync(TestContext.Current.CancellationToken);
        savedEntities.ShouldSatisfyAllConditions(
            se => se.Count.ShouldBe(expectedCount, "we should not seed duplicates"),
            se => se.Select(e => e.Type)
                .Distinct()
                .Count()
                .ShouldBe(expectedCount, "each facility type should have a unique type"));
    }

    [Fact]
    public async Task Seed_SpecifyingCustomisedEntityWithHardcodedIds_IsSeededSuccessfully()
    {
        _context.Seed(new FacilityTypeEntitySeed()
            .With(f => f.Type, FacilityType.TennisCourt)
            .Without(f => f.Description));

        var savedEntities = await _context.Set<FacilityTypeEntity>().ToListAsync(TestContext.Current.CancellationToken);

        savedEntities.ShouldSatisfyAllConditions(
            se => se.Count.ShouldBe(1, "we should only seed one entity"),
            se => se.First().Type.ShouldBe(FacilityType.TennisCourt, "the seeded entity should have the correct type"),
            se => se.First().Description.ShouldBeNull("the seeded entity should not have a description"));
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction.Dispose();
    }
}