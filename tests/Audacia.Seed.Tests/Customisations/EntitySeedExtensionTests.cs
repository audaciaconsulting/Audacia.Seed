using System.Diagnostics.CodeAnalysis;
using Audacia.Seed.Customisation;
using Audacia.Seed.EntityFrameworkCore.Extensions;
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

namespace Audacia.Seed.Tests.Customisations;

[Collection(CollectionNames.TestDatabaseContextCollection)]
public sealed class EntitySeedExtensionTests : IDisposable
{
    private readonly TestDatabaseContext _context;
    private readonly IDbContextTransaction _transaction;

    public EntitySeedExtensionTests()
    {
        _context = TestDatabaseContextBuilder.CreateContext();

        _transaction = _context.Database.BeginTransaction();
    }

    [Fact]
    public async Task With_SpecifiesPropertyOverrides_ValuesAreSetOnSavedData()
    {
        // Check we can specify arbitrary properties on the entity.
        const MembershipLevel expectedMembershipLevel = MembershipLevel.Gold;
        var expectedFirstName = Guid.NewGuid().ToString();
        var seedConfiguration = new MemberSeed()
            .With(m => m.FirstName, expectedFirstName)
            .With(m => m.MembershipLevel, expectedMembershipLevel);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Member>().FirstAsync();
        using (new AssertionScope())
        {
            savedEntity.MembershipLevel.Should().Be(
                expectedMembershipLevel,
                $"we should be able to set the {nameof(Member.MembershipLevel)} on the entity using {nameof(EntitySeedExtensions.With)}");
            savedEntity.FirstName.Should().Be(
                expectedFirstName,
                $"we should be able to set the {nameof(Member.FirstName)} on the entity using {nameof(EntitySeedExtensions.With)}");
        }
    }

    [Fact]
    public async Task With_SpecifiesPropertyOverridesForParent_ValuesAreSetOnSavedData()
    {
        // Check we can specify arbitrary properties on a parent entity.
        var expectedFirstName = Guid.NewGuid().ToString();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member.FirstName, expectedFirstName);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Booking>().Include(m => m.Member).FirstAsync();
        savedEntity.Member.FirstName.Should()
            .Be(expectedFirstName, "we should be able to set properties on required parents");
    }

    [Fact]
    public async Task With_NewEntityForNavigationProperty_OverridesDefaultSeededData()
    {
        var membershipGroup = _context.Seed(new MembershipGroupSeed());
        var expectedFirstName = Guid.NewGuid().ToString();
        var seedConfiguration = new BookingSeed().With(b => b.Member, new Member
        {
            FirstName = expectedFirstName,
            LastName = Guid.NewGuid().ToString(),
            MembershipGroupId = membershipGroup.Id
        });
        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Booking>().Include(b => b.Member).FirstAsync();
        savedEntity.Member.FirstName.Should().Be(
            expectedFirstName,
            "we should be able to override the default parent navigation property using a plain instance of the parent");
    }

    [Fact]
    public async Task With_OverridingPrerequisiteWithParent_OnlyOneAddedToTheDatabase()
    {
        var membershipGroup = _context.Seed(new MembershipGroupSeed());
        var expectedFirstName = Guid.NewGuid().ToString();
        var seedConfiguration = new BookingSeed().With(b => b.Member, new Member
        {
            FirstName = expectedFirstName,
            LastName = Guid.NewGuid().ToString(),
            MembershipGroupId = membershipGroup.Id
        });
        _context.Seed(seedConfiguration);

        var members = await _context.Set<Member>().ToListAsync();
        using (new AssertionScope())
        {
            members.Should().HaveCount(
                1,
                "overriding a prerequisite should mean the default prerequisite isn't seeded");
            members.Single().FirstName.Should().Be(
                expectedFirstName,
                "we should have overridden the prerequisite based on the provided entity");
        }
    }

    [Fact]
    public void With_ForPropertyOnOptionalNavigation_ThrowsException()
    {
        var seedConfiguration = new BookingSeed()
            .With(b => b.Coupon!.Name, "I should null ref");

        var act = () => _context.Seed(seedConfiguration);

        act.Should().ThrowExactly<DataSeedingException>(
                "we should show a more useful error message if we catch a null reference exception when applying customisations")
            // Make sure the exception message is helpful to the developer.
            .WithMessage($"*{nameof(Coupon)}*{nameof(Coupon.Name)}*nullable property*");
    }

    [Fact]
    public async Task With_ForForeignKeyOnOptionalNavigationProperty_OnlyOneNavigationPropertySeeded()
    {
        var couponId = _context.Seed<Coupon>().Id;
        var seed = new BookingSeed().With(b => b.CouponId, couponId);

        var booking = _context.Seed(seed);

        var coupons = await _context.Set<Coupon>().Include(c => c.Booking).ToListAsync();
        using (new AssertionScope())
        {
            coupons.Should().HaveCount(1, "we should have seeded one coupon");
            coupons.Single().Booking!.Id.Should().Be(
                booking.Id,
                "we should have set the foreign key on the optional navigation property");
        }
    }

    [Fact]
    public void With_ForForeignKeyOnRequiredNavigationProperty_OnlyOneNavigationPropertySeeded()
    {
        var facilityId = _context.Seed<Facility>().Id;
        var seed = new BookingSeed().With(b => b.FacilityId, facilityId);

        var booking = _context.Seed(seed);

        var facilities = _context.Set<Facility>().Include(f => f.Bookings).ToList();
        using (new AssertionScope())
        {
            facilities.Should().HaveCount(1, "we should have seeded one facility");
            facilities.Single().Bookings.Should().ContainSingle(
                b => b.Id == booking.Id,
                "we should have set the foreign key on the required navigation property");
        }
    }

    [Fact]
    public async Task With_ForOptionalNavigation_PopulatedInTheDatabase()
    {
        var roomName = Guid.NewGuid().ToString();
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Room, new Room { Name = roomName, Region = new() });

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Facility>().Include(b => b.Room).FirstAsync();
        savedEntity.Room.Should().NotBeNull();
        savedEntity.Room?.Name.Should()
            .Be(
                roomName,
                $"it should use the raw entity passed into the {nameof(EntitySeedExtensions.With)} method");
    }

    [Fact]
    public async Task With_ManyEntitiesCreatedWithTheSameProperty_AllEntitiesHaveTheSamePropertyValue()
    {
        const int expectedCount = 10;
        var expectedName = Guid.NewGuid().ToString();
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Name, expectedName);

        _context.SeedMany(expectedCount, seedConfiguration);

        var savedEntities = await _context.Set<Facility>().ToListAsync();
        savedEntities.All(m => m.Name == expectedName).Should()
            .BeTrue(
                $"{nameof(EntitySeedExtensions.With)} should set the same property value on each entity if only one is specified");
    }

    [Fact]
    public async Task With_DelegateProvidedForPropertySetterBasedOnIndex_EntitiesHavePropertiesSetByTheDelegate()
    {
        const int expectedCount = 5;
        var seedConfiguration = new CouponSeed()
            .With(c => c.Discount, index => index * 10m);

        _context.SeedMany(expectedCount, seedConfiguration);

        decimal[] expectedDiscounts = [0, 10m, 20m, 30m, 40m];
        var savedEntities = await _context.Set<Coupon>().ToListAsync();
        savedEntities.Select(m => m.Discount).Should().BeEquivalentTo(
            expectedDiscounts,
            $"{nameof(EntitySeedExtensions.With)} should use the provided delegate to set the property values based on the index");
    }

    [Fact]
    public async Task With_DelegateProvidedForPropertySetterBasedOnPrevious_EntitiesHavePropertiesSetByTheDelegate()
    {
        const int expectedCount = 5;
        var seedConfiguration = new CouponSeed()
            .With(c => c.Discount, (_, previous) => previous?.Discount * 2 ?? 0.1m);

        _context.SeedMany(expectedCount, seedConfiguration);

        decimal[] expectedDiscounts = [0.1m, 0.2m, 0.4m, 0.8m, 1.6m];
        var savedEntities = await _context.Set<Coupon>().ToListAsync();
        savedEntities.Select(m => m.Discount).Should().BeEquivalentTo(
            expectedDiscounts,
            $"{nameof(EntitySeedExtensions.With)} should use the provided delegate to set the property values based on the previous");
    }

    [Fact]
    public async Task With_DelegateProvidedForPropertySetter_EachEntityHasUniqueValue()
    {
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Name, () => Guid.NewGuid().ToString());

        const int amountToCreate = 5;
        _context.SeedMany(amountToCreate, seedConfiguration);

        var savedEntities = await _context.Set<Facility>().ToListAsync();
        savedEntities.Select(f => f.Name).Should().OnlyHaveUniqueItems(
            $"{nameof(EntitySeedExtensions.With)} should use the provided delegate to set the property values");
    }

    [Fact]
    public async Task With_ManyEntitiesCreatedWithDifferentProperties_AllEntitiesHaveThePropertiesAsSpecified()
    {
        string[] expectedNames = ["Facility 1", "Facility 2"];
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Name, expectedNames);

        _context.SeedMany(expectedNames.Length, seedConfiguration);

        var savedEntities = await _context.Set<Facility>().ToListAsync();
        savedEntities.DistinctBy(c => c.Name).Should().HaveCount(
            expectedNames.Length,
            $"we should set the properties in the same order as they appear in the {nameof(expectedNames)} array");
    }

    [Fact]
    public void With_FewerValuesProvidedThanWeWantToSeed_ThrowsException()
    {
        string[] names = ["Facility 1", "Facility 2"];
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Name, names);

        // Seed one more than the name we provided.
        var amountToCreate = names.Length + 1;
        var act = () => _context.SeedMany(amountToCreate, seedConfiguration);

        act.Should()
            .ThrowExactly<DataSeedingException>(
                "we should show a developer-friendly message for this as we cannot give a build error for it")
            .WithMessage($"We are building {amountToCreate} entities of type {nameof(Facility)}, but {names.Length} were provided.");
    }

    [Fact]
    public void With_MoreValuesProvidedThanWeWantToSeed_ThrowsException()
    {
        string[] names = ["Facility 1", "Facility 2", "Facility 3"];
        var seedConfiguration = new FacilitySeed()
            .With(f => f.Name, names);

        // Seed one less than the name we provided.
        var amountToCreate = names.Length - 1;
        var act = () => _context.SeedMany(amountToCreate, seedConfiguration);

        act.Should()
            .ThrowExactly<DataSeedingException>(
                "we should show a developer-friendly message for this as we cannot give a build error for it")
            .WithMessage($"We are building {amountToCreate} entities of type {nameof(Facility)}, but {names.Length} were provided.");
    }

    [Fact]
    public void With_AlreadySeededGrandparentForeignKeyPropertyIsSet_DoesNotSeedExtraEntitiesWhenSettingForeignKey()
    {
        var existingMembershipGroup = _context.Seed<MembershipGroup>();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member.MembershipGroupId, existingMembershipGroup.Id);

        _context.Seed(seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(1, "we should not seed an extra membership group");
    }

    [Fact]
    public void With_AlreadySeededGrandparentNavigationPropertyIsSet_DoesNotSeedExtraEntitiesWhenSettingNavigationProperty()
    {
        var existingMembershipGroup = _context.Seed<MembershipGroup>();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member.MembershipGroup, existingMembershipGroup);

        _context.Seed(seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(1, "we should not seed an extra membership group");
    }

    [Fact]
    public void With_AlreadySeededGrandparentForeignKeyPropertiesAreSet_DoesNotSeedExtraEntitiesWhenSettingForeignKey()
    {
        var existingMembershipGroups = _context.SeedMany<MembershipGroup>(2).ToList();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member.MembershipGroupId, existingMembershipGroups[0].Id, existingMembershipGroups[1].Id);

        _context.SeedMany(2, seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(2, "we should not seed an extra membership group");
    }

    [Fact]
    public void With_AlreadySeededGrandparentNavigationPropertiesAreSet_DoesNotSeedExtraEntitiesWhenSettingNavigationProperty()
    {
        var existingMembershipGroups = _context.SeedMany<MembershipGroup>(2).ToList();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member.MembershipGroup, existingMembershipGroups[0], existingMembershipGroups[1]);

        _context.SeedMany(2, seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(2, "we should not seed an extra membership group");
    }

    [Fact]
    public void With_AlreadySeededParentNavigationPropertyIsSet_DoesNotSeedExtraEntitiesWhenSettingNavigationProperty()
    {
        var existingMember = _context.Seed<Member>();
        var seedConfiguration = new BookingSeed()
            .With(b => b.Member, existingMember);

        _context.Seed(seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(1, "we should not seed an extra member");
    }

    [Fact]
    public void With_AlreadySeededParentForeignKeyPropertyIsSet_DoesNotSeedExtraEntitiesWhenSettingForeignKey()
    {
        var existingMember = _context.Seed<Member>();
        var seedConfiguration = new BookingSeed()
            .With(b => b.MemberId, existingMember.Id);

        _context.Seed(seedConfiguration);

        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        membershipGroups.Should().HaveCount(1, "we should not seed an extra member");
    }

    [Fact]
    public void With_AlreadySeededParentNavigationPropertyIsSetWithLambda_DoesNotSeedExtraEntitiesWhenSettingNavigationProperty()
    {
        var asset = _context.Seed(new EntitySeed<EmployeeAsset>());

        var entitySeed = new EntitySeed<CompanyAsset>()
            .With(b => b.Asset, () => asset);
        _context.Seed(entitySeed);

        var membershipGroups = _context.Set<EmployeeAsset>().ToList();
        membershipGroups.Should().HaveCount(1, "we should not seed an extra employee asset");
    }

    [Fact]
    public void With_ExplicitlyCastingToCorrectType_CastedEntityIsSeeded()
    {
        var seed = new EntitySeed<CompanyAsset>()
            .WithNew(ca => ca.Asset, new EntitySeed<EmployeeAsset>())
            .With(ca => ((EmployeeAsset)ca.Asset).Employee.FirstName, "John");

        _context.Seed(seed);

        var companyAsset = _context.Set<CompanyAsset>()
            .Include(ca => ca.Asset)
            .ThenInclude(ca => ((EmployeeAsset)ca).Employee)
            .Single();
        var singleAssetInTheDatabase = _context.Set<Asset>()
            .Single();
        using (new AssertionScope())
        {
            companyAsset.Asset.Should().BeOfType<EmployeeAsset>();
            companyAsset.Asset.Should().Be(singleAssetInTheDatabase);
            ((EmployeeAsset)companyAsset.Asset).Employee.FirstName.Should().Be("John");
        }
    }

    [Fact]
    public async Task WithNew_SpecifiesNewSeedForNavigationProperty_OverridesDefaultSeededData()
    {
        const string expectedName = "Squash court 2";
        var seedConfiguration = new BookingSeed().WithNew(
            b => b.Facility,
            new FacilitySeed()
                .With(f => f.Name, expectedName));

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Booking>().Include(b => b.Facility).FirstAsync();
        savedEntity.Facility.Name.Should().Be(
            expectedName,
            "we should be able to override the default parent navigation property");
    }

    [Fact]
    public async Task WithNew_NewSeedForOptionalNavigation_OptionalPropertyIsSet()
    {
        var seedConfiguration = new FacilitySeed()
            .WithNew(f => f.Room);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Facility>().Include(b => b.Room).FirstAsync();
        savedEntity.Room.Should()
            .NotBeNull("we should be able to specify a parent for an optional navigation property");
    }

    [Fact]
    public async Task WithNew_SeedClassExistsInProject_UsesSeedClass()
    {
        var seed = new FacilitySeed()
            .WithNew(f => f.Room);

        var seededEntity = _context.Seed(seed);

        var room = await _context.Set<Room>().SingleAsync(b => b.Id == seededEntity.RoomId);
        room.Name.Should()
            .NotBeNullOrWhiteSpace(
                $"we should use the {nameof(RoomSeed)} if it is not provided for the {nameof(EntitySeedExtensions.WithNew)} method");
    }

    [Fact]
    public void WithNew_SeedClassDoesNotExistInProjectForParent_CanBeSeeded()
    {
        var seed = new RoomSeed()
            .WithNew(r => r.Region)
            .With(r => r.Name, "Test Room");

        var seededEntity = _context.Seed(seed);

        seededEntity.RegionId.Should().BeGreaterThan(
            0,
            $"we should be able to seed the {nameof(Room.Region)} without a dedicated entity seed class");
    }

    [Fact]
    public async Task WithNew_OverridesPrerequisite_OnlyOneAddedToTheDatabase()
    {
        var expectedFirstName = Guid.NewGuid().ToString();
        var seed = new BookingSeed()
            .WithNew(b => b.Member, new MemberSeed().With(m => m.FirstName, expectedFirstName));

        _context.Seed(seed);

        var members = await _context.Set<Member>().ToListAsync();
        using (new AssertionScope())
        {
            members.Should().HaveCount(
                1,
                "overriding a prerequisite should mean the default prerequisite isn't seeded");
            members.Single().FirstName.Should().Be(
                expectedFirstName,
                "we should have overridden the prerequisite based on the provided entity");
        }
    }

    [Fact]
    public async Task WithNew_OverridesPrerequisite_ChildrenShareTheSameParent()
    {
        var seed = new BookingSeed()
            .WithNew(b => b.Member, new MemberSeed().With(m => m.FirstName, "First name"));

        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, seed);

        var bookings = await _context.Set<Booking>().ToListAsync();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(
                amountToCreate,
                "overriding a prerequisite should mean the default prerequisite isn't seeded");
            bookings.Select(m => m.MemberId).Distinct().Should().HaveCount(
                1,
                "child entities should share a parent even if the seed prerequisite is customised");
        }
    }

    [Fact]
    public async Task WithNew_DefaultSeed_ChildrenShareTheSameParent()
    {
        const int amountToCreate = 2;
        _context.SeedMany<Booking>(amountToCreate);

        var bookings = await _context.Set<Booking>().ToListAsync();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(
                amountToCreate,
                "we should have seeded the correct number of bookings");
            bookings.Select(m => m.MemberId).Distinct().Should().HaveCount(
                1,
                "child entities should share a parent when using the default seed");
        }
    }

    [Fact]
    public async Task WithNew_OverridesPrerequisiteWithCustomisations_ChildrenShareTheSameParent()
    {
        var expectedFirstName = Guid.NewGuid().ToString();
        var seed = new BookingSeed()
            .WithNew(b => b.Member, new MemberSeed().With(m => m.FirstName, expectedFirstName));

        const int amountToCreate = 5;
        _context.SeedMany(amountToCreate, seed);

        var bookings = await _context.Set<Booking>().ToListAsync();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(
                amountToCreate,
                "overriding a prerequisite should mean the default prerequisite isn't seeded");
            bookings.Select(m => m.MemberId).Distinct().Should().HaveCount(
                1,
                "child entities should share a parent even if the seed prerequisite is customised");
        }
    }

    [Fact]
    public async Task
        WithNew_OverridesPrerequisiteWhenThereAreMultiplePropertiesOfTheSameType_CorrectNumberOfEntitiesSeeded()
    {
        var expectedOwnerName = Guid.NewGuid().ToString();
        var expectedManagerName = Guid.NewGuid().ToString();
        var seed = new FacilitySeed()
            // Overriding the owner & member should create two entities
            .WithNew(f => f.Owner, new EmployeeSeed().With(m => m.FirstName, expectedOwnerName))
            .WithNew(f => f.Manager, new EmployeeSeed().With(m => m.FirstName, expectedManagerName));

        _context.Seed(seed);

        var employees = await _context.Set<Employee>()
            .Include(e => e.FacilitiesOwned)
            .Include(e => e.FacilitiesManaged)
            .ToListAsync();
        using (new AssertionScope())
        {
            const int expectedNumberOfEmployees = 2;
            employees.Should().HaveCount(
                expectedNumberOfEmployees,
                $"we should have seeded two employees for the {nameof(Facility.Owner)} and {nameof(Facility.Manager)} respectively");
            employees.Single(e => e.FacilitiesOwned.Any()).FirstName.Should()
                .BeEquivalentTo(expectedOwnerName, "we should have set the name on the owner");
            employees.Single(e => e.FacilitiesManaged.Any()).FirstName.Should().BeEquivalentTo(
                expectedManagerName,
                "we should have set the name on the manager");
        }
    }

    [Fact]
    public async Task WithNew_MultiplePrerequisitesProvided_EachChildHasItsOwnParent()
    {
        const int amountToCreate = 3;
        var seedConfiguration = new FacilitySeed()
            .WithNew(
                f => f.Room,
                new RoomSeed().With(r => r.Name, "Room 1"),
                new RoomSeed().With(r => r.Name, "Room 2"),
                new RoomSeed().With(r => r.Name, "Room 3"));

        _context.SeedMany(amountToCreate, seedConfiguration);

        string[] expectedRoomNames = ["Room 1", "Room 2", "Room 3"];
        var roomNames = await _context.Set<Facility>().Select(f => f.Room!.Name).ToListAsync();
        roomNames.Should().BeEquivalentTo(
            expectedRoomNames,
            "information from each seed should be used with specifying many prerequisites");
    }

    [Fact]
    public async Task WithNew_SeedClassDoesExistsInProject_UsesSeedClass()
    {
        var seed = new EntitySeed<Facility>()
            .With(f => f.Name, "Test facility") // This is just so it can be saved.
            .WithNew(f => f.Room)
            .WithNew(f => f.TypeEntity);

        var seededEntity = _context.Seed(seed);

        var room = await _context.Set<Room>().SingleAsync(b => b.Id == seededEntity.RoomId);
        room.Name.Should()
            .NotBeNullOrWhiteSpace($"we should have used the {nameof(RoomSeed)} for the Room");
    }

    [Fact]
    public void WithNew_SeedClassDoesNotExistInProject_UsesDefaultSeed()
    {
        var seed = new EntitySeed<Room>()
            .WithNew(r => r.Region)
            .With(r => r.Name, "Test room");

        var seededEntity = _context.Seed(seed);

        seededEntity.RegionId.Should().BeGreaterThan(
            0,
            $"we should be able to seed the {nameof(Room.Region)} without a dedicated entity seed class");
    }

    [Fact]
    public void WithNew_Grandparent_CanBeSeeded()
    {
        var seed = new EntitySeed<Booking>()
            .WithNew(b => b.Facility.Room);

        var seededEntity = _context.Seed(seed);

        var booking = _context.Set<Booking>().Include(b => b.Facility.Room).Single(b => b.Id == seededEntity.Id);

        booking.Facility.Room.Should().NotBeNull(
            "we should be able to seed grandparents as a prerequisite");
    }

    [Fact]
    public void WithNew_ParentAlreadyInChangeTrackerWhenSeedingMany_SeedsNewParent()
    {
        // This adds a Member because it's a required parent
        _context.Seed<Booking>();
        // This pulls a Member into the change tracker
        var existingMember = _context.Set<Member>().Single();
        // This asks for a new member to be set up regardless
        var seed = new BookingSeed().WithNew(b => b.Member);

        var bookings = _context.SeedMany(2, seed).ToList();

        using (new AssertionScope())
        {
            bookings.All(b => b.MemberId != existingMember.Id).Should().BeTrue(
                "we should ignore entities in the change tracker when using WithNew and seeding many entities");
            bookings.Select(b => b.MemberId).Distinct().Should().HaveCount(
                1,
                "each new booking should have the same new member");
        }
    }

    [Fact]
    public void WithNew_ParentAlreadyInChangeTracker_SeedsNewParent()
    {
        // This adds a Member because it's a required parent
        _context.Seed<Booking>();
        // This pulls a Member into the change tracker
        var existingMember = _context.Set<Member>().Single();
        // This asks for a new member to be set up regardless
        var seed = new BookingSeed().WithNew(b => b.Member);

        var booking = _context.Seed(seed);

        booking.MemberId.Should().NotBe(existingMember.Id, "we should ignore entities in the change tracker when using WithNew");
    }

    [Fact]
    public void WithNew_GrandparentAlreadyInChangeTracker_SeedsNewGrandparent()
    {
        _context.Seed<Booking>();
        var existingGroup = _context.Set<MembershipGroup>().Single();
        var seed = new EntitySeed<Booking>()
            .WithNew(b => b.Member.MembershipGroup);

        var seededEntity = _context.Seed(seed);

        var booking = _context.Set<Booking>().Include(b => b.Member).Single(b => b.Id == seededEntity.Id);
        booking.Member.MembershipGroupId.Should().NotBe(existingGroup.Id, "we should ignore entities in the change tracker when using WithNew");
    }

    [Fact]
    public void WithNew_FewerValuesProvidedThanWeWantToSeed_ThrowsException()
    {
        RoomSeed[] prerequisites = [new RoomSeed(), new RoomSeed(), new RoomSeed()];
        var seedConfiguration = new FacilitySeed()
            .WithNew(f => f.Room, prerequisites);

        // Seed one more than the name we provided.
        var amountToCreate = prerequisites.Length + 1;
        var act = () => _context.SeedMany(amountToCreate, seedConfiguration);

        act.Should()
            .ThrowExactly<DataSeedingException>(
                "we should show a developer-friendly message for this as we cannot give a build error for it")
            .WithMessage($"We are building {amountToCreate} entities of type {nameof(Facility)}, but {prerequisites.Length} were provided.");
    }

    [Fact]
    public void WithNew_MoreValuesProvidedThanWeWantToSeed_ThrowsException()
    {
        RoomSeed[] prerequisites = [new RoomSeed(), new RoomSeed(), new RoomSeed()];
        var seedConfiguration = new FacilitySeed()
            .WithNew(f => f.Room, prerequisites);

        // Seed one less than the name we provided.
        var amountToCreate = prerequisites.Length - 1;
        var act = () => _context.SeedMany(amountToCreate, seedConfiguration);

        act.Should()
            .ThrowExactly<DataSeedingException>(
                "we should show a developer-friendly message for this as we cannot give a build error for it")
            .WithMessage($"We are building {amountToCreate} entities of type {nameof(Facility)}, but {prerequisites.Length} were provided.");
    }

    [Fact]
    public async Task WithChildren_UsedForChildNavigationProperties_ParentHasChildrenSeeded()
    {
        const int numberOfChildren = 5;
        var seedConfiguration = new MemberSeed()
            .WithChildren(m => m.Bookings, numberOfChildren);

        var member = _context.Seed(seedConfiguration);

        var countOfParentsSeeded = await _context.Set<Member>().CountAsync();
        var countOfChildrenSeeded = await _context.Set<Booking>().CountAsync(b => b.MemberId == member.Id);
        using (new AssertionScope())
        {
            countOfParentsSeeded.Should().Be(
                1,
                $"{nameof(EntitySeedExtensions.WithChildren)} should only seed one parent entity");
            countOfChildrenSeeded.Should().Be(
                numberOfChildren,
                $"{nameof(EntitySeedExtensions.WithChildren)} should seed the specified number of children for the parent entity");
        }
    }

    [Fact]
    public async Task WithChildren_EachChildHasItsOwnSeed_ParentHasBothDifferentChildrenSeeded()
    {
        const int numberOfChildren = 2;
        var seedConfiguration = new RoomSeed()
            .WithChildren(
                r => r.Facilities,
                numberOfChildren,
                new FacilitySeed().With(f => f.Name, "Facility 1", "Facility 2"));

        _context.Seed(seedConfiguration);

        var childrenSeeded = await _context.Set<Facility>().ToListAsync();
        using (new AssertionScope())
        {
            childrenSeeded.Select(c => c.Name).Should().BeEquivalentTo(
                new[] { "Facility 1", "Facility 2" },
                "we should be able to customise children on a per-seed basis");
        }
    }

    [Fact]
    public async Task WithExisting_ForOptionalNavigation_PopulatedInTheDatabase()
    {
        var roomName = Guid.NewGuid().ToString();
        _context.Seed(new RoomSeed().With(r => r.Name, roomName));
        var seedConfiguration = new FacilitySeed().WithExisting(f => f.Room);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Facility>().Include(f => f.Room).FirstAsync();
        savedEntity.Room?.Name.Should()
            .Be(roomName, "it should select an existing Room from the database");
    }

    [Fact]
    public async Task WithChildren_BuildingManyOptionalParents_EachParentGetsItsOwnChildren()
    {
        const int numberOfChildren = 3;
        var seedConfiguration = new RoomSeed()
            .WithChildren(
                r => r.Facilities,
                numberOfChildren,
                new FacilitySeed().With(f => f.Name, "Facility 1", "Facility 2", "Facility 3"));

        const int amountToSeed = 2;
        _context.SeedMany(amountToSeed, seedConfiguration);

        var childrenSeeded = await _context.Set<Facility>().ToListAsync();
        using (new AssertionScope())
        {
            childrenSeeded.Select(c => c.Name).Should().BeEquivalentTo(
                new[] { "Facility 1", "Facility 2", "Facility 3", "Facility 1", "Facility 2", "Facility 3" },
                "we should be able to customise the child seed that each optional parent receives a copy of");
        }
    }

    [Fact]
    public async Task WithChildren_BuildingManyRequiredParents_EachParentGetsItsOwnChildren()
    {
        const int numberOfChildren = 3;
        var seedConfiguration = new FacilitySeed()
            .WithChildren(
                f => f.Bookings,
                numberOfChildren,
                new BookingSeed().With(b => b.Notes, "Booking 1", "Booking 2", "Booking 3"));

        const int amountToSeed = 2;
        _context.SeedMany(amountToSeed, seedConfiguration);

        var childrenSeeded = await _context.Set<Booking>().ToListAsync();
        using (new AssertionScope())
        {
            childrenSeeded.Select(c => c.Notes).Should().BeEquivalentTo(
                new[] { "Booking 1", "Booking 2", "Booking 3", "Booking 1", "Booking 2", "Booking 3" },
                "we should be able to customise the child seed that each required parent receives a copy of");
        }
    }

    [Fact]
    public async Task WithChildren_SeedingMany_EachEntityHasTheSameNumberOfChildren()
    {
        const int numberOfChildren = 5;
        var seedConfiguration = new RoomSeed()
            .WithChildren(r => r.Facilities, numberOfChildren);

        const int amountToSeed = 3;
        _context.SeedMany(amountToSeed, seedConfiguration);

        var childrenSeeded = await _context.Set<Facility>().ToListAsync();
        using (new AssertionScope())
        {
            var facilitiesPerRoom = childrenSeeded.GroupBy(c => c.RoomId).ToList();
            facilitiesPerRoom.Should().HaveCount(
                amountToSeed,
                $"{nameof(DbContextExtensions.SeedMany)} should the number of entities provided");
            facilitiesPerRoom.All(b => b.Count() == numberOfChildren).Should()
                .BeTrue(
                    $"{nameof(EntitySeedExtensions.WithChildren)} should seed the specified number of children for each parent entity");
            childrenSeeded.Should().HaveCount(
                numberOfChildren * amountToSeed,
                $"{nameof(EntitySeedExtensions.WithChildren)} should seed the specified number of children for the parent entity");
        }
    }

    [Fact]
    public async Task WithChildren_EachChildHasDifferentNames_EachChildReceivesCorrectName()
    {
        var facilityNames = new[] { "Facility 1", "Facility 2", "Facility 3" };
        var childSeed = new FacilitySeed()
            .With(f => f.Name, facilityNames);
        var seed = new RoomSeed()
            .WithChildren(r => r.Facilities, facilityNames.Length, childSeed);

        const int roomsToSeed = 2;
        _context.SeedMany(roomsToSeed, seed);

        var facilities = await _context.Set<Facility>().ToListAsync();
        using (new AssertionScope())
        {
            var facilitiesPerRoom = facilities.GroupBy(b => b.RoomId).ToList();
            facilitiesPerRoom.Should().HaveCount(
                roomsToSeed,
                "we should have seeded the correct number of rooms");
            foreach (var facilitiesForBooking in facilitiesPerRoom)
            {
                facilitiesForBooking.Select(f => f.Name).Should().BeEquivalentTo(
                    facilityNames,
                    "each child should have the correct name");
            }
        }
    }

    [Fact]
    public async Task WithExisting_PredicateProvided_UsesCorrectEntity()
    {
        const int amountToCreate = 3;
        const string expectedRoomName = "Room 3";
        var roomSeed = new RoomSeed().With(r => r.Name, "Room 1", "Room 2", expectedRoomName);
        _context.SeedMany(amountToCreate, roomSeed);
        var seedConfiguration = new FacilitySeed().WithExisting(f => f.Room, r => r.Name == expectedRoomName);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Facility>().Include(f => f.Room).FirstAsync();
        savedEntity.Room?.Name.Should()
            .Be(expectedRoomName, "it should select the correct Room from the database");
    }

    [Fact]
    public async Task WithExisting_OverridesPrerequisite_PrerequisiteNotSeeded()
    {
        var roomName = Guid.NewGuid().ToString();
        _context.Seed(new RoomSeed().With(b => b.Name, roomName));
        var seedConfiguration = new FacilitySeed().WithExisting(f => f.Room);

        _context.Seed(seedConfiguration);

        var savedEntity = await _context.Set<Facility>().Include(f => f.Room).FirstAsync();
        savedEntity.Room!.Name.Should().Be(roomName, "it should select the Room from the database");
    }

    [Fact]
    public async Task WithExisting_OverridesPrerequisite_OnlyOneAddedToTheDatabase()
    {
        var expectedMemberName = Guid.NewGuid().ToString();
        _context.Seed(new MemberSeed().With(m => m.FirstName, expectedMemberName));
        var seed = new BookingSeed().WithExisting(b => b.Member);

        _context.Seed(seed);

        var members = await _context.Set<Member>().ToListAsync();
        using (new AssertionScope())
        {
            members.Should().HaveCount(
                1,
                "overriding a prerequisite should mean the default prerequisite isn't seeded");
            members.Single().FirstName.Should().Be(
                expectedMemberName,
                "we should have overridden the prerequisite based on the provided entity");
        }
    }

    [Fact]
    public async Task WithDifferent_ForRequiredParent_EachChildHasItsOwnParent()
    {
        const int amountToCreate = 10;
        var seedConfiguration = new BookingSeed()
            .WithDifferent(b => b.Member);

        _context.SeedMany(amountToCreate, seedConfiguration);

        const int expectedCount = amountToCreate;
        var savedEntities = await _context.Set<Booking>().ToListAsync();
        savedEntities.DistinctBy(b => b.MemberId).Should()
            .HaveCount(
                expectedCount,
                $"{nameof(EntitySeedExtensions.WithDifferent)} should give each child its own parent");
    }

    [Fact]
    public async Task WithDifferent_WithPropertyOverrides_EachParentHasDifferentPropertyValue()
    {
        const int amountToCreate = 3;
        var seedConfiguration = new FacilitySeed()
            .WithDifferent(f => f.Room)
            .With(f => f.Room!.Name, "First name", "Second name", "Third name");

        _context.SeedMany(amountToCreate, seedConfiguration);

        var roomNames = await _context.Set<Facility>()
            .Select(f => f.Room!.Name)
            .ToListAsync();
        roomNames.Should().BeEquivalentTo(
            ["First name", "Second name", "Third name"],
            "we should be able to set different values for each parent");
    }

    // It's easy to unintentionally change (i.e break) the behaviour depending on the amount being created, so test a few values to make sure this hasn't happened.
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task WithDifferent_ForGreatGrandParent_EachParentHasDifferentPropertyValue(int amountToCreate)
    {
        var seedConfiguration = new BookingSeed()
            .WithDifferent(b => b.Member.MembershipGroup.Region);

        _context.SeedMany(amountToCreate, seedConfiguration);

        var bookings = await _context.Set<Booking>()
            .Include(b => b.Member.MembershipGroup)
            .ToListAsync();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(amountToCreate, $"we should have seeded {amountToCreate} {nameof(Booking)}s");
            bookings.Select(b => b.MemberId).Distinct().Should().HaveCount(
                amountToCreate,
                $"we should have seeded {amountToCreate} {nameof(Member)}s");
            bookings.Select(b => b.Member.MembershipGroupId).Distinct().Should().HaveCount(
                amountToCreate,
                $"we should have seeded {amountToCreate} {nameof(MembershipGroup)}s");
            bookings.Select(b => b.Member.MembershipGroup.RegionId).Distinct().Should().HaveCount(
                amountToCreate,
                $"we should have seeded {amountToCreate} {nameof(Region)}s");
            _context.Set<Member>().Should().HaveCount(amountToCreate);
            _context.Set<MembershipGroup>().Should().HaveCount(amountToCreate);
            _context.Set<Region>().Should().HaveCount(amountToCreate);
        }
    }

    // It's easy to unintentionally change (i.e break) the behaviour depending on the amount being created, so test a few values to make sure this hasn't happened.
    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public async Task WithDifferent_ForGrandParent_EachParentHasDifferentPropertyValue(int amountToCreate)
    {
        var seedConfiguration = new BookingSeed()
            .WithDifferent(b => b.Member.MembershipGroup);

        _context.SeedMany(amountToCreate, seedConfiguration);

        var bookings = await _context.Set<Booking>()
            .Include(b => b.Member.MembershipGroup)
            .ToListAsync();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(amountToCreate, $"we should have seeded {amountToCreate} {nameof(Booking)}s");
            bookings.Select(b => b.MemberId).Distinct().Should().HaveCount(
                amountToCreate,
                $"we should have seeded {amountToCreate} {nameof(Member)}s");
            bookings.Select(b => b.Member.MembershipGroupId).Distinct().Should().HaveCount(
                amountToCreate,
                $"we should have seeded {amountToCreate} {nameof(MembershipGroup)}s");
            _context.Set<Member>().Should().HaveCount(amountToCreate);
            _context.Set<MembershipGroup>().Should().HaveCount(amountToCreate);
        }
    }

    [Fact]
    public async Task Without_ForOptionalProperty_PropertyIsSetToNull()
    {
        var seedConfiguration = new BookingSeed()
            .Without(b => b.Notes);

        _context.Seed(seedConfiguration);

        var savedEntities = await _context.Set<Booking>().SingleAsync();
        savedEntities.Notes.Should().BeNull($"{nameof(EntitySeedExtensions.Without)} should null out the property");
    }

    [Fact]
    public void WithPrimaryKey_IncorrectPropertyTypeProvided_ExceptionThrown()
    {
        var seed = new EntitySeed<Booking>()
            .WithPrimaryKey(Guid.NewGuid().ToString());

        var act = () => _context.Seed(seed);

        act.Should()
            .ThrowExactly<DataSeedingException>("we should not be able to set the incorrect type for the primary key");
    }

    [Fact]
    public void WithPrimaryKey_EntityHasCompositeKey_ExceptionThrown()
    {
        var seed = new EntitySeed<CouponIssuer>()
            .WithPrimaryKey(1);

        var act = () => _context.Seed(seed);

        act.Should()
            .ThrowExactly<DataSeedingException>("we should not be able to do this for entities with composite keys");
    }

    [Fact]
    public void WithPrimaryKey_ExampleProvided_SavesThePrimaryKeyCorrectly()
    {
        // Arbitrary primary key value
        const int firstPimaryKeyValue = 12834238;
        var firstSeed = new EntitySeed<Booking>()
            .WithPrimaryKey(firstPimaryKeyValue);
        var firstBooking = _context.Seed(firstSeed);

        // See if we can seed another one with a lower int PK
        const int secondPrimaryKeyValue = 11111;
        var secondSeed = new EntitySeed<Booking>()
            .WithPrimaryKey(secondPrimaryKeyValue);
        var secondBooking = _context.Seed(secondSeed);

        using (new AssertionScope())
        {
            firstBooking.Id.Should().Be(firstPimaryKeyValue);
            secondBooking.Id.Should().Be(secondPrimaryKeyValue);
        }
    }

    [Fact]
    public void WithPrimaryKey_SeedingMany_SavesThePrimaryKeyCorrectly()
    {
        // Arbitrary primary key value
        int[] primaryKeyValues = [50, 55, 12];
        var firstSeed = new EntitySeed<Booking>()
            .WithPrimaryKey(primaryKeyValues);

        var bookings = _context.SeedMany(3, firstSeed);

        using (new AssertionScope())
        {
            bookings.Select(b => b.Id).Should().BeEquivalentTo(primaryKeyValues);
        }
    }

    [Fact]
    public void BookingBelongsToFacilitiesWhichMayNotBeInTheSameRoom_DataSeededCorrectly()
    {
        var room = _context.Seed<Room>();
        var seed = new BookingSeed()
            .WithDifferent(b => b.Facility)
            .With(b => b.Facility.RoomId, room.Id, null, room.Id);

        const int bookingsToSeed = 3;
        var bookings = _context.SeedMany(bookingsToSeed, seed).ToList();

        bookings.Select(b => b.FacilityId).Distinct().Should().HaveCount(bookingsToSeed);
        var roomIds = bookings.ConvertAll(b => b.Facility.RoomId);
        roomIds.Should()
            .BeEquivalentTo(
                new int?[] { room.Id, null, room.Id },
                "we should be able to seed different rooms for each facility");
    }

    [Fact]
    public async Task FacilitiesHaveDifferentNumbersOfBookings_DataSeededCorrectly()
    {
        var facilitySeeds = new[] { new FacilitySeed(), new FacilitySeed() };
        var seed = new BookingSeed()
            .WithNew(b => b.Facility, facilitySeeds[0], facilitySeeds[0], facilitySeeds[1]);

        const int bookingsToSeed = 3;
        _context.SeedMany(bookingsToSeed, seed);

        var facilities = await _context.Set<Facility>().Include(f => f.Bookings).ToListAsync();
        using (new AssertionScope())
        {
            facilities.Should().HaveCount(2, "we should have seeded two facilities");
            facilities.Count(f => f.Bookings.Count == 1).Should()
                .Be(1, "we should have seeded a facility with a single booking");
            facilities.Count(f => f.Bookings.Count == 2).Should()
                .Be(1, "we should have seeded a facility with two bookings");
        }
    }

    [Fact]
    public void MemberHasConflictingBookingsInDifferentRooms_DataSeededCorrectly()
    {
        var start = DateTime.Now;
        var finish = start.AddHours(1);
        var seed = new BookingSeed()
            .With(b => b.Start, start, start)
            .With(b => b.Finish, finish, finish)
            .WithDifferent(b => b.Facility.Room);

        const int amountToCreate = 2;
        var bookings = _context.SeedMany(amountToCreate, seed).ToList();

        bookings.Select(b => b.FacilityId).Distinct().Should()
            .HaveCount(amountToCreate, "the bookings should be for different facilities");
        bookings.Select(b => b.Facility.RoomId).Distinct().Should()
            .HaveCount(amountToCreate, "the facilities should be in different rooms");
    }

    [Fact]
    public async Task FacilitiesAreOwnedAndManagedByTwoDifferentEmployees_DataSeededCorrectly()
    {
        // Employee A manages Room A and owns Room B
        // Employee B manages Room B and owns Room A
        var seedEmployeeA = new EmployeeSeed().With(e => e.FirstName, "A");
        var seedEmployeeB = new EmployeeSeed().With(e => e.FirstName, "B");
        var seed = new FacilitySeed()
            .With(f => f.Name, "A", "B")
            .WithNew(b => b.Owner, seedEmployeeB, seedEmployeeA)
            .WithNew(b => b.Manager, seedEmployeeA, seedEmployeeB);

        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, seed);

        var employees = await _context.Set<Employee>()
            .Include(e => e.FacilitiesManaged)
            .Include(e => e.FacilitiesOwned)
            .ToListAsync();
        var employeeA = employees.Single(e => e.FirstName == "A");
        var employeeB = employees.Single(e => e.FirstName == "B");
        using (new AssertionScope())
        {
            employeeA.FacilitiesManaged.Select(f => f.Name).Should().BeEquivalentTo("A");
            employeeA.FacilitiesOwned.Select(f => f.Name).Should().BeEquivalentTo("B");
            employeeB.FacilitiesManaged.Select(f => f.Name).Should().BeEquivalentTo("B");
            employeeB.FacilitiesOwned.Select(f => f.Name).Should().BeEquivalentTo("A");
        }
    }

    [Fact]
    public async Task FacilitiesAreOwnedAndManagedByThreeDifferentEmployees_DataSeededCorrectly()
    {
        // Employee A manages Room A and Room B
        // Employee B manages Room C and Owns Room A
        // Employee C owns Room B and Room C
        var seedEmployeeA = new EmployeeSeed().With(e => e.FirstName, "A");
        var seedEmployeeB = new EmployeeSeed().With(e => e.FirstName, "B");
        var seedEmployeeC = new EmployeeSeed().With(e => e.FirstName, "C");
        var seed = new FacilitySeed()
            .With(f => f.Name, "A", "B", "C")
            .WithNew(b => b.Owner, seedEmployeeB, seedEmployeeC, seedEmployeeC)
            .WithNew(b => b.Manager, seedEmployeeA, seedEmployeeA, seedEmployeeB);

        const int amountToCreate = 3;
        _context.SeedMany(amountToCreate, seed);

        var employees = await _context.Set<Employee>()
            .Include(e => e.FacilitiesManaged)
            .Include(e => e.FacilitiesOwned)
            .ToListAsync();
        var employeeA = employees.Single(e => e.FirstName == "A");
        var employeeB = employees.Single(e => e.FirstName == "B");
        var employeeC = employees.Single(e => e.FirstName == "C");
        using (new AssertionScope())
        {
            employeeA.FacilitiesManaged.Select(f => f.Name).Should().BeEquivalentTo("A", "B");
            employeeA.FacilitiesOwned.Select(f => f.Name).Should().BeEmpty();
            employeeB.FacilitiesManaged.Select(f => f.Name).Should().BeEquivalentTo("C");
            employeeB.FacilitiesOwned.Select(f => f.Name).Should().BeEquivalentTo("A");
            employeeC.FacilitiesManaged.Select(f => f.Name).Should().BeEmpty();
            employeeC.FacilitiesOwned.Select(f => f.Name).Should().BeEquivalentTo("B", "C");
        }
    }

    [Fact]
    public async Task TwoMembersHaveTwoBookingsEach_DataSeededCorrectly()
    {
        const int bookingsPerMember = 2;
        var seed = new MemberSeed()
            .WithChildren(m => m.Bookings, bookingsPerMember);

        const int amountOfMembers = 2;
        _context.SeedMany(amountOfMembers, seed);

        var members = await _context.Set<Member>().ToListAsync();
        var bookings = await _context.Set<Booking>().ToListAsync();
        using (new AssertionScope())
        {
            members.Should().HaveCount(amountOfMembers, "we should have seeded two members");
            foreach (var bookingsForMember in bookings.GroupBy(b => b.MemberId))
            {
                bookingsForMember.Should().HaveCount(bookingsPerMember, "each member should have 2 bookings");
            }
        }
    }

    [Fact]
    public async Task SettingDifferentOptionalNavigationsAsPartOfSameSave_DataSeededCorrectly()
    {
        var poolFacilitySeed = new FacilitySeed().ForPool();
        var poolBookingSeed = new BookingSeed()
            .WithNew(b => b.Facility, poolFacilitySeed)
            .With(b => b.Facility.Pool!.Name, "Pool 1");
        var roomFacilitySeed = new FacilitySeed().ForRoom();
        var roomBookingSeed = new BookingSeed()
            .WithNew(b => b.Facility, roomFacilitySeed)
            .With(b => b.Facility.Room!.Name, "Room 1");

        _context.Seed(poolBookingSeed, roomBookingSeed);

        var bookings = await _context.Set<Booking>()
            .Include(b => b.Facility)
            .ToListAsync();
        const int expectedCount = 2;
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(expectedCount);
            bookings.Count(b => b.Facility is { PoolId: not null, RoomId: null }).Should()
                .Be(1, "one of the bookings should be in a pool");
            bookings.Count(b => b.Facility is { PoolId: null, RoomId: not null }).Should()
                .Be(1, "one of the bookings should be in a room");
        }
    }

    [Fact]
    [SuppressMessage("Maintainability", "ACL1002: Methods should not exceed a predefined number of statements",
        Justification = "We are testing a complicated data setup & need to perform many asserts as a result.")]
    public void MultipleWithDifferentSpecified_IsNotOverwritten()
    {
        var entitySeed = new BookingSeed()
            .WithDifferent(b => b.Member.MembershipGroup)
            .WithDifferent(b => b.Facility.Owner)
            .WithDifferent(b => b.Facility.Pool);

        const int bookingsToCreate = 2;
        _context.SeedMany(bookingsToCreate, entitySeed);

        var bookingAfterSave = _context.Set<Booking>()
            .Include(b => b.Member.MembershipGroup)
            .Include(b => b.Facility.Owner)
            .Include(b => b.Facility.Pool)
            .ToList();
        var ownerIds = bookingAfterSave.ConvertAll(b => b.Facility.OwnerId).Distinct().ToList();
        var poolIds = bookingAfterSave.ConvertAll(b => b.Facility.PoolId).Distinct().ToList();
        var membershipGroupIds = bookingAfterSave.ConvertAll(b => b.Member.MembershipGroupId).Distinct().ToList();
        var members = _context.Set<Member>().ToList();
        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        var facilities = _context.Set<Facility>().ToList();
        var owners = _context.Set<Employee>().Where(e => e.FacilitiesOwned.Any()).ToList();
        var pools = _context.Set<Pool>().ToList();

        using (new AssertionScope())
        {
            ownerIds.Should().HaveCount(bookingsToCreate, "each booking should have a different owner");
            poolIds.Should().HaveCount(bookingsToCreate, "each booking should have a different pool");
            membershipGroupIds.Should().HaveCount(
                bookingsToCreate,
                "each booking's member should be in a distinct membership group");
            members.Should().HaveCount(bookingsToCreate, "each booking should have a different member");
            membershipGroups.Should()
                .HaveCount(bookingsToCreate, "there should be two membership groups in the database");
            facilities.Should().HaveCount(bookingsToCreate, "there should be two facilities in the database");
            owners.Should().HaveCount(
                bookingsToCreate,
                "there should be two owners (excluding managers) in the database");
            pools.Should().HaveCount(bookingsToCreate, "there should be two pools in the database");
        }
    }

    [Fact]
    public void WithDifferentSpecifiedAfterWithExistingForSameParent_AppliesWithDifferentFirst()
    {
        _context.Seed<Employee>();
        var entitySeed = new BookingSeed()
            // The order we specify this in should not matter
            .WithExisting(b => b.Facility.Manager)
            .WithDifferent(b => b.Facility.Room);

        const int bookingsToCreate = 2;
        _context.SeedMany(bookingsToCreate, entitySeed);

        var bookingAfterSave = _context.Set<Booking>()
            .Include(b => b.Facility.Manager)
            .Include(b => b.Facility.Room)
            .ToList();
        var managerIds = bookingAfterSave.ConvertAll(b => b.Facility.ManagerId).Distinct().ToList();
        var facilityIds = bookingAfterSave.ConvertAll(b => b.FacilityId).Distinct().ToList();
        var roomIds = bookingAfterSave.ConvertAll(b => b.Facility.RoomId).Distinct().ToList();
        using (new AssertionScope())
        {
            managerIds.Should().HaveCount(1);
            facilityIds.Should().HaveCount(2);
            roomIds.Should().HaveCount(2);
        }
    }

    [Fact]
    public void SeedingManyEntitiesWithCompositeKey_DataSeededWithUniqueKeyInformation()
    {
        var entitySeed = new EntitySeed<CouponIssuer>();

        const int amountToCreate = 2;
        var couponIssuers = _context.SeedMany(amountToCreate, entitySeed).ToList();

        using (new AssertionScope())
        {
            couponIssuers.Select(ci => ci.CouponId).Distinct().Should().HaveCount(amountToCreate);
            couponIssuers.Select(ci => ci.IssuerId).Distinct().Should().HaveCount(amountToCreate);
        }
    }

    [Fact]
    public void SpecifyingUniqueValuesForCompositeKey_DataSeededCorrectly()
    {
        const int amountToCreate = 2;
        var coupons = _context.SeedMany<Coupon>(amountToCreate).ToList();
        var employees = _context.SeedMany<Employee>(amountToCreate).ToList();
        var entitySeed = new EntitySeed<CouponIssuer>()
            .With(ci => ci.CouponId, coupons[0].Id, coupons[1].Id)
            .With(ci => ci.IssuerId, employees[0].Id, employees[1].Id);

        var couponIssuers = _context.SeedMany(amountToCreate, entitySeed).ToList();

        using (new AssertionScope())
        {
            couponIssuers.Select(ci => ci.CouponId).Should().ContainInOrder(coupons[0].Id, coupons[1].Id);
            couponIssuers.Select(ci => ci.IssuerId).Should().ContainInOrder(employees[0].Id, employees[1].Id);
        }
    }

    [Fact]
    public void SpecifyingNonUniqueValuesForCompositeKey_ExceptionThrown()
    {
        var coupon = _context.Seed<Coupon>();
        var employee = _context.Seed<Employee>();
        var entitySeed = new EntitySeed<CouponIssuer>()
            .With(ci => ci.CouponId, coupon.Id)
            .With(ci => ci.IssuerId, employee.Id);

        const int amountToCreate = 2;
        var act = () => _context.SeedMany(amountToCreate, entitySeed).ToList();

        act.Should().Throw<Exception>("we should not be able to seed multiple entities with the same composite key");
    }

    [Fact]
    public void SeedingDifferentEntityBetweenMultiStageSeeding_DataSavedCorrectly()
    {
        var facility = _context.Seed<Facility>();
        // This is a bugfix. Previously, this cleared the change tracker (including the facility above), so thought we were hardcoding the id of the facility used below.
        _context.Seed<Facility>();
        var bookingSeed = new BookingSeed().With(b => b.Facility, facility);

        var booking = _context.Seed(bookingSeed);

        booking.FacilityId.Should().Be(facility.Id);
    }

    [Fact]
    public void SpecifyingExplicitIdForGrandparentInOrder_CanBeSetCorrectly()
    {
        const int managersNeeded = 2;
        var employees = _context.SeedMany<Employee>(managersNeeded).ToList();
        var bookingSeed = new EntitySeed<Booking>()
            .With(b => b.Facility.ManagerId, employees[0].Id, employees[0].Id, employees[1].Id);
        const int amountToCreate = 3;
        _context.SeedMany(amountToCreate, bookingSeed);

        var allBookings = _context.Set<Booking>().Include(b => b.Facility).ToList();
        var allManagers = _context.Set<Employee>().Where(e => e.FacilitiesManaged.Any()).ToList();
        using (new AssertionScope())
        {
            allBookings.Select(b => b.Facility.ManagerId).Should()
                .BeEquivalentTo(
                    [allManagers[0].Id, allManagers[0].Id, allManagers[1].Id],
                    "the Manager Ids should be set as specified in the seed configuration.");
            allManagers.Should().HaveCount(managersNeeded, "we should not have seeded more employees");
        }
    }

    [Fact]
    public void SpecifyingExplicitIdForGreatGrandparentInOrder_CanBeSetCorrectly()
    {
        const int groupsNeeded = 2;
        var groups = _context.SeedMany<MembershipGroup>(groupsNeeded).ToList();
        var bookingSeed = new EntitySeed<Booking>()
            .With(b => b.Member.MembershipGroup.ParentId, groups[0].Id, groups[0].Id, groups[1].Id);
        _context.SeedMany(3, bookingSeed);

        var bookingsAfterSave = _context.Set<Booking>().Include(b => b.Member.MembershipGroup).ToList();
        using (new AssertionScope())
        {
            bookingsAfterSave.Select(b => b.Member.MembershipGroup.ParentId).Should()
                .BeEquivalentTo(
                    [groups[0].Id, groups[0].Id, groups[1].Id],
                    "the Membership Group Ids should be set as specified in the seed configuration.");
        }
    }

    [Fact]
    public void SpecifyingExplicitPropertyForParentInOrder_ParentPropertiesArePopulatedWithProvidedValues()
    {
        var bookingSeed = new EntitySeed<Booking>()
            .WithDifferent(b => b.Member)
            .With(b => b.Member.FirstName, "John", "Jane");
        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, bookingSeed);

        using (new AssertionScope())
        {
            var members = _context.Set<Member>().ToList();
            members.Should().HaveCount(
                amountToCreate,
                "we should overwrite the default seed doing a WithDifferent explicitly");
            var bookingsAfterSave = _context.Set<Booking>().Include(b => b.Member).ToList();
            bookingsAfterSave.Select(b => b.Member.FirstName).Should()
                .BeEquivalentTo(["John", "Jane"]);
        }
    }

    [Fact]
    public void SpecifyingExplicitPropertyForGrandparentInOrder_DoesNotSeedMoreDataThanItShould()
    {
        var bookingSeed = new EntitySeed<Booking>()
            .WithDifferent(b => b.Member.MembershipGroup)
            .With(b => b.Member.MembershipGroup.Name, "Group 1", "Group 2");
        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, bookingSeed);

        using (new AssertionScope())
        {
            var members = _context.Set<Member>().ToList();
            members.Should().HaveCount(
                amountToCreate,
                "we should overwrite the default seed doing a WithDifferent explicitly");
            var groups = _context.Set<MembershipGroup>().ToList();
            groups.Should().HaveCount(
                amountToCreate,
                "we should overwrite the default seed doing a WithDifferent explicitly");
            var bookingsAfterSave = _context.Set<Booking>().Include(b => b.Member.MembershipGroup).ToList();
            bookingsAfterSave.Select(b => b.Member.MembershipGroup.Name).Should()
                .BeEquivalentTo(["Group 1", "Group 2"]);
        }
    }

    [Fact]
    public void SpecifyingExplicitIdForGrandparentToTheSameValue_DoesNotSeedMoreDataThanItShould()
    {
        var membershipGroup = _context.Seed<MembershipGroup>();

        var bookingSeed = new EntitySeed<Booking>()
            .With(b => b.Member.MembershipGroupId, membershipGroup.Id);
        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, bookingSeed);

        using (new AssertionScope())
        {
            var members = _context.Set<Member>().ToList();
            members.Should().HaveCount(1);
            var groups = _context.Set<MembershipGroup>().ToList();
            groups.Should().HaveCount(1);
            var bookingsAfterSave = _context.Set<Booking>().Include(b => b.Member.MembershipGroup).ToList();
            bookingsAfterSave.Should().HaveCount(2);
            bookingsAfterSave.Select(b => b.Member.MembershipGroupId).Should()
                .BeEquivalentTo([membershipGroup.Id, membershipGroup.Id]);
        }
    }

    [Fact]
    public void SpecifyingExplicitPropertyForGrandparentToTheSameValue_DoesNotSeedMoreDataThanItShould()
    {
        var membershipGroup = _context.Seed<MembershipGroup>();

        var bookingSeed = new EntitySeed<Booking>()
            .With(b => b.Member.MembershipGroup, membershipGroup);
        const int amountToCreate = 2;
        _context.SeedMany(amountToCreate, bookingSeed);

        using (new AssertionScope())
        {
            var members = _context.Set<Member>().ToList();
            members.Should().HaveCount(1);
            var groups = _context.Set<MembershipGroup>().ToList();
            groups.Should().HaveCount(1);
            var bookingsAfterSave = _context.Set<Booking>().Include(b => b.Member.MembershipGroup).ToList();
            bookingsAfterSave.Should().HaveCount(2);
            bookingsAfterSave.Select(b => b.Member.MembershipGroupId).Should()
                .BeEquivalentTo([membershipGroup.Id, membershipGroup.Id]);
        }
    }

    [Fact]
    public void SeedingChildrenWithDifferentBaseClassInheritors_CanChildrenOfDifferentTypes()
    {
        IEntitySeed<Asset>[] seeds =
        [
            new EntitySeed<EmployeeAsset>(),
            new EntitySeed<PoolAsset>(),
            new EntitySeed<RoomAsset>()
        ];

        var seed = new EntitySeed<CompanyAsset>()
            .WithNew(ca => ca.Asset, seeds);
        _context.SeedMany(3, seed);

        using (new AssertionScope())
        {
            var savedAssets = _context.Set<Asset>().ToList();
            savedAssets.Should().HaveCount(3, "we should have seeded three assets of different types");
            savedAssets.OfType<EmployeeAsset>().Should().HaveCount(1, $"one of the assets should be of type {nameof(EmployeeAsset)}");
            savedAssets.OfType<PoolAsset>().Should().HaveCount(1, $"one of the assets should be of type {nameof(PoolAsset)}");
            savedAssets.OfType<RoomAsset>().Should().HaveCount(1, $"one of the assets should be of type {nameof(RoomAsset)}");
        }
    }

    [Fact]
    public void MultipleWithCallsPartiallyMatchTheGetter_NumberOfParentsSeededReflectsTheNumberOfPropertiesProvided()
    {
        // These .With calls contain some parents in commons (i.e Member), but diverge afterwards.
        _context.SeedMany(2, new BookingSeed()
            .With(b => b.Member.FirstName, "John", "Jane")
            .With(b => b.Member.MembershipGroup.Name, "Group 1", "Group 2"));

        var bookings = _context.Set<Booking>().ToList();
        var members = _context.Set<Member>().ToList();
        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(2);
            members.Should().HaveCount(2);
            membershipGroups.Should().HaveCount(2);

            members.All(m => m.Bookings.Count == 1).Should().BeTrue();
            membershipGroups.All(m => m.Members.Count == 1).Should().BeTrue();
        }
    }

    [Fact]
    public void MultipleWithDifferentCallsPartiallyMatchTheGetter_NumberOfParentsSeededReflectsTheNumberOfPropertiesProvided()
    {
        _context.SeedMany(2, new BookingSeed()
            .WithDifferent(b => b.Member)
            .WithDifferent(b => b.Member.MembershipGroup));

        var bookings = _context.Set<Booking>().ToList();
        var members = _context.Set<Member>().ToList();
        var membershipGroups = _context.Set<MembershipGroup>().ToList();
        using (new AssertionScope())
        {
            bookings.Should().HaveCount(2);
            members.Should().HaveCount(2);
            membershipGroups.Should().HaveCount(2);

            members.All(m => m.Bookings.Count == 1).Should().BeTrue();
            membershipGroups.All(m => m.Members.Count == 1).Should().BeTrue();
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        _transaction.Dispose();
    }
}