# Audacia.Seed

## Overview
`Audacia.Seed` is an open-source library for seeding test data into a repository. It has support for the following ORMs:

- Entity Framework Core (`Audacia.Seed.EntityFrameworkCore`)
- Entity Framework (`Audacia.Seed.EntityFramework`)

The aim of this library is to reduce the overhead of setting up data for testing. It provides a fluent API for seeding data, and supports customisation of the data being seeded. 
This library has taken inspiration from libraries such as [Bogus](https://github.com/bchavez/Bogus) and [AutoFixture](https://github.com/AutoFixture/AutoFixture), with the following key differences.
- **Conservative seeding**: children will share a parent by default. Parents that are deemed to be optional will not be seeded unless otherwise specified.
- **ORM-centric approach**: All data seeding actions are into a provided `ISeedableRepository`, rather than generating POCOs on the fly.

This library is recommended for seeding complex data scenarios, and not appropriate for generating test data that isn't immediately saved.

### The problem

For even the simplest of data models, a lot of boilerplate code is required to seed a valid entity:
````cs 
var booking = new Booking
{
    //... set any required fields    
    Facility = new Facility 
    {
        //... set any required fields
    },
    Member = new Member
    {
        //... set any required fields
    }
};
_context.Set<Booking>().Add(booking);
_context.SaveChanges();
````

### The solution
`Audacia.Seed` removes this verbosity with the following:
```csharp
_context.Seed<Booking>();
```
Similarly, if multiple entities need to be seeded:
```csharp
const int amountOfBookingsToSeed = 5;
_context.SeedMany<Booking>(amountOfBookingsToSeed);
```

### How does it work?

Inheritors of `EntitySeed<TEntity>` are searched for using reflection. If none can be found, the library will attempt to create a valid instance of `TEntity` on its own (see [Seeding without a class](#seeding-without-a-class)).

## The `EntitySeed<TEntity>` class
The `EntitySeed<TEntity>` class provides a way of creating a default `TEntity` to be seeded into the database.
It should seed _exactly_ the information required for an instance of `TEntity` to be considered valid. For example, it may be that you have two optional navigation properties, but your domain ensures exactly one is populated. This is a good use-case for a custom class as the default behaviour will be to seed neither of these navigation properties.

You can create a class that inherits from `EntitySeed<TEntity>` to represent a single valid instance of `TEntity` as below: 
```csharp
public class BookingSeed : EntitySeed<Booking>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Booking, Facility>(f => f.Facility),
        new SeedPrerequisite<Booking, Member>(f => f.Member)
    ];

    protected override Booking GetDefault(int index, Booking? previous)
    {
        return new Booking
        {
            Notes = Guid.NewGuid().ToString()
        };
    }
}
```

### Seeding without a class
It's possible to seed an entity without a dedicated class for it. In this scenario, the library will seed the entity by attempting to create a valid instance on it's own.
#### `Prerequisites` default behaviour
When seeding without a class, we figure out the `Prerequisites` as follows:
1. The library finds all fields that it thinks are navigation properties. This is based on the property being a `class`, and the existance of another property with the name `{PropertyName}Id`.
2. A navigation property is considered as mandatory if its corresponding foreign key is not nullable.
3. For each mandatory navigation property, we try to find a `EntitySeed<TNavigation>` class for it. If none is found, we'll repeat this process for `TNavigation`.

#### `GetDefault` default behaviour
When seeding without a class, the library will call the first constructor it finds for `TEntity`, using example values for any constructor arguments as needed:
- if it is a `string`, a random `Guid` will be used.
- otherwise, the default value for the type will be used.

### When to override `Prerequisites` / `GetDfault`
Both the `Prerequisites` and `GetDefault` methods are optional. If you don't need to explicitly state what this behaviour will be, the library will behave [as above](#seeding-without-a-class).

`Prerequisites` should be overridden if any mandatory properties don't follow the naming convention outlined above, or the navigation properties need to look a certain way by default. For example, a `Booking` must have a `Member` with a specific `MembershipLevel`.

`GetDefault` should be overridden if creating a `new TEntity()`, and immediately saving throws errors that aren't related to foreign keys (e.g a required `string` field). For example, a `Coupon` must have a non-zero `discount`.

## Customisation
At a high level:

```csharp
// When you want to override any default behaviour to specify a value that is meaningful to the test.
new BookingSeed().With(b => b.Name, "Booking Name");

// When you want to override any default behaviour to set a property as null.
new BookingSeed().Without(b => b.Name);

// When your database already contains the data you want to join up to.
new BookingSeed().WithExisting(b => b.Member);

// When you want each child to have a different parent.
new BookingSeed().WithDifferent(b => b.Member);

// When you want to specify an optional navigation property.
new BookingSeed().WithPrerequisite(b => b.Coupon);

// When you want to override the default behaviour for a mandatory navigation property.
new BookingSeed().WithPrerequisite(b => b.Member, new MemberSeed().With(m => m.FirstName, "Member Name"));

//  When you're seeding from the parent level, but want to ensure the children are set up.
new MemberSeed().WithChildren(m => m.Bookings);
```

Depending on whether you're seeding one or many entities, these customisations can then be seeded into the database context as follows:

```csharp
// When you're seeding one entity
_context.Seed(seed);

// When you're seeding more than one entity
_context.SeedMany(5, seed);
```

Entities can be customised within the test via the following:
### `With` (single entity)
#### Set properties on an entity:
```csharp
new BookingSeed()
    .With(b => b.Name, "Booking Name")
    .With(b => b.StartDate, new DateTime(2024, 1, 1));
```
_Note: this can be also done for navigation properties so long as you provide a valid instance of the navigation property._

#### Set a property on a parent entity:
```csharp
new BookingSeed().With(m => m.Member.FirstName, "Member Name");
```
#### Set a navigation property:
```csharp
new BookingSeed().With(m => m.Member, new Member() {...});
```

### `With` (multiple entities)
The following examples apply to when you're seeding multiple entities (i.e with `SeedMany`).

#### Set a property to a single value for all entities:
```csharp
new BookingSeed().With(b => b.Name, "Booking Name");
```
Note: in this scenario, all entities will have the same value for the property.

#### Set a property to multiple values in order:
```csharp
new BookingSeed().With(b => b.Name, "Booking A", "Booking B", "Booking C");
```

#### Set a property based on the index of the entity being seeded
```csharp
new CouponSeed().With(c => c.Discount, index => index * 10m);
```

#### Set a property based on the previous entity:
```csharp
new CouponSeed().With(c => c.Discount, (_, previous) => previous?.Discount * 2 ?? 0.1m)
```

#### Set a property with a new value each time:
```csharp
new CouponSeed().With(
    scs => scs.Name,
    // Delegate is re-evaluated each time
    () => Guid.NewGuid().ToString())
```
### `Without`

#### Set a property on an entity as `null`;
```csharp
new BookingSeed().Without(b => b.Name);
```


### `WithPrerequisite` (single entity)
Specify that an navigation optional property should be non-null, or override the default behaviour for a mandatory navigation property.
#### Set a navigation property without a seed:
```csharp
new BookingSeed().WithPrerequisite(b => b.Coupon);
```
In this scenario, we will look for inheritors of `EntitySeed<Coupon>`, falling back on the default behaviour if none are found (see [Seeding without a class](#seeding-without-a-class)).

#### Set a navigation property with a seed
```csharp
new BookingSeed().WithPrerequisite(b => b.Coupon, new CouponSeed().With(...));
```

#### Seed a grandparent entity
```csharp
new BookingSeed().WithPrerequisite(b => b.Facility.Building);
```
Note that this isn't necessary if these relationships are mandatory.

### `WithPrerequisite` (multiple entities)

#### Set a navigation property to the provided seeds in order
```csharp
List<MemberSeed> memberSeeds = [ new MemberSeed(), new MemberSeed() ];

new BookingSeed().WithPrerequisite(
    b => b.Coupon,
    // First 3 bookings are for the first member
    memberSeeds[0], 
    memberSeeds[0], 
    memberSeeds[0], 
    // Remaining 2 bookings are for the second member
    memberSeeds[1], 
    memberSeeds[1]);
```
Note that if all you need is for the parents to be different, you can use [WithDifferent](#withdifferent-multiple-entities) instead.

#### Set a navigation property to the same value for all entities:
```csharp
new BookingSeed().WithPrerequisite(
    // All bookings get the same member with the provided first name.
    b => b.Member, 
    new MemberSeed().With(m => m.FirstName, "Member Name"));
```

### `WithDifferent` (multiple entities)
You can only use `WithDifferent` if you are seeding more than one entity. As such, the library will throw an exception if passed into `Seed` rather than `SeedMany`.
#### Specify that two entities should not share a parent:
```csharp
new BookingSeed().WithDifferent(b => b.Member);
```
#### Specify that two entities should not share a grandparent (and therefore parent):
```csharp
new BookingSeed().WithDifferent(b => b.Member.MembershipGroup);
```

### `WithExisting`
Use `WithExisting` to seed an entity that already exists in the database. Use this when you've already seeded other entities, or you're testing against a readily-seeded database.

#### Set a navigation property to an existing entity from the database
```csharp
new BookingSeed().WithExisting(b => b.Member);
```

#### Set a navigation property to an existing entity from the database based on a predicate
```csharp
new BookingSeed().WithExisting(b => b.Member, m => m.FirstName == "Member Name");
```

### `WithChildren`
#### Set a child navigation property
This will seed a single child `Booking` for each `Member`.
```csharp
new MemberSeed().WithChildren(m => m.Bookings);
```

#### Set a child navigation property to a collection of the provided size

```csharp
const int numberOfBookings = 5;
new MemberSeed().WithChildren(m => m.Bookings, numberOfBookings);
```

#### Set a child navigation property to a collection of the provided size, using an `EntitySeed<TChild>`

```csharp
const int numberOfBookings = 2;
new MemberSeed().WithChildren(
    m => m.Bookings, 
    numberOfBookings,
    new BookingSeed().With(b => b.Name, "First Booking", "Second Booking"));
```

Note that you cannot use `WithChildren` to join up to existing data in the database (like with `WithExisting`). You can use the `With` extension method to do this:
```csharp
var existingBooking = _context.Bookings.First();
new MemberSeed().With(m => m.Bookings, [existingBooking]);
```

### Add your own customisation
Consider the following interface:
```csharp
public interface IHasName
{
    string Name { get; set; }
}
```
You can specify a customisation to set these fields automatically as follows:
```csharp
public class SeedNamesCustomisation<TEntity> : ISeedCustomisation<TEntity>
    where TEntity : class, IHasName
{
    public void Apply(TEntity entity, ISeedableRepository repository, int index, TEntity? previous)
    {
        entity.Name = Guid.NewGuid().ToString();
    }
}
```

This customisation can then be applied to any entity that implements `IHasName` using the `AddCustomisation` method:
```csharp
new PersonSeed().AddCustomisation(new SeedNamesCustomisation<Person>())
```
And will therefore automatically set the `Name` property of all `Member`s.


### More complex seeding scenarios

#### Bookings are for different facilities, some of which are in the same building
```csharp
var building = _context.Seed<Building>();
var seed = new BookingSeed()
    .WithDifferent(b => b.Facility)
    .With(b => b.Facility.BuildingId, building.Id, null, building.Id);

const int bookingsToSeed = 3;
var bookings = _context.SeedMany(bookingsToSeed, seed).ToList();

// Seeds the following:
// > Facility 1 (in Building 1)
//     > Booking 1
// > Facility 2 (no building)
//     > Booking 2
// > Facility 3 (in Building 1)
//     > Booking 3
```

#### Bookings belong to different facilities, some of which are the same. 
```csharp
var facilitySeeds = new[] { new FacilitySeed(), new FacilitySeed() };
var seed = new BookingSeed()
    .WithPrerequisite(b => b.Facility, facilitySeeds[0], facilitySeeds[0], facilitySeeds[1]);

const int bookingsToSeed = 3;
_context.SeedMany(bookingsToSeed, seed);

// Seeds the following:
// > Facility 1
//     > Booking 1
//     > Booking 2
// > Facility 2
//     > Booking 3
```

#### A member has a conflicting booking in a different building
```csharp
var start = DateTime.Now;
var finish = start.AddHours(1);
var seed = new BookingSeed()
    .With(b => b.Start, start)
    .With(b => b.Finish, finish)
    .WithDifferent(b => b.Facility.Building);

const int amountToCreate = 2;
var bookings = _context.SeedMany(amountToCreate, seed).ToList();

// Seeds the following:
// > Building 1
//     > Facility 1
//         > Booking 1 (Start: Now, Finish: Now + 1 hour)
// > Building 2
//     > Facility 2
//         > Booking 2 (Start: Now, Finish: Now + 1 hour)
```

#### Facilities are owned and managed by different employees
```csharp
// Employee A manages Facility A and Facility B
// Employee B manages Facility C and Owns Facility A
// Employee C owns Facility B and Facility C
var employeeSeeds = new[]
{
    new EmployeeSeed().With(e => e.FirstName, "A"),
    new EmployeeSeed().With(e => e.FirstName, "B"),
    new EmployeeSeed().With(e => e.FirstName, "C")
};
var seed = new FacilitySeed()
    .With(f => f.Name, "A", "B", "C")
    .WithPrerequisite(f => f.Owner, employeeSeeds[1], employeeSeeds[2], employeeSeeds[2])
    .WithPrerequisite(f => f.Manager, employeeSeeds[0], employeeSeeds[0], employeeSeeds[1]);

const int amountToCreate = 3;
_context.SeedMany(amountToCreate, seed);

// Seeds the following:
// > Facility A (Owned by Employee B, Managed by Employee A)
// > Facility B (Owned by Employee C, Managed by Employee A)
// > Facility C (Owned by Employee C, Managed by Employee B)
```

#### Two members have two bookings each
```csharp
const int bookingsPerMember = 2;
var seed = new MemberSeed()
    .WithChildren(m => m.Bookings, bookingsPerMember);

const int amountOfMembers = 2;
_context.SeedMany(amountOfMembers, seed);

// Seeds the following:
// > Member 1
//     > Booking 1
//     > Booking 2
// > Member 2
//     > Booking 3
//     > Booking 4
```

## Use seeds from a separate project
By default, the library will look for seeds in the same assembly as the test. If you have a separate project for your seeds, you can specify this assembly attribute in your test project as follows:
```csharp
[assembly: SeedAssembly("My.Seed.Assembly")]
```
See [here](./tests/Audacia.Seed.Tests/AssemblyAttributes.cs) for an example.

## Usage notes

### `WithMany` mismatches

We don't know at compile time how many entities you want to seed vs how many entities your customisation will handle. As such, the following code will build successfully:
```csharp
var seed = new BookingSeed()
    .With(b => b.Name, "Booking A", "Booking B");

_context.SeedMany(3, seed); 
```
In this scenario, a `DataSeedingException` will be thrown. This will also happen if you provide 3 values, but only 2 entities are being seeded.

Note that you can still provide a single value for a property, and seed multiple entities. In this scenario, all entities will have the same value for the property.


### Children share parents by default.
When seeding many children, the default behaviour will be to share a parent.

```csharp
_context.SeedMany<Booking>(5);

_context.Set<Booking>.Count();// 5 Bookings
_context.Set<Facility>.Count();// 1 Facility for all Bookings
_context.Set<Member>.Count();// 1 Member for all Bookings
```
This is also true for optional navigations. 

To override this behaviour, use [WithDifferent](#withdifferent-multiple-entities):
```csharp
_context.SeedMany(
    5, 
    new BookingSeed()
        .WithDifferent(b => b.Member)
        .WithDifferent(b => b.Facility));

_context.Set<Booking>.Count();// 5 Bookings
_context.Set<Facility>.Count();// 5 Facilities, one for each Booking
_context.Set<Member>.Count();// 5 Members, one for each Booking
```

## Pass one seed per branch of the data model into `WithSeeds`

If you need to seed data for a complex graph of objects with multiple branches, a good way to approach this is to work out the nearest entity which all branches will seed. The seed configuration for that entity will be reused by default, so you can use one seed configuration per branch, and they should all join up at that entity.

For example, consider this data model:
- A `Booking` has a single `Member`.
- A `Member` has many `Contact`s, which has many `Address`es.

It is difficult to seed everything from the `Address` level, because it is dependent on the `Member`, but the `Member` is the principal in the other relationships being seeded, so you end up seeding from the principal end of the relationship.

Instead, provide one seed configuration for each branch of the data model. In this case, we have two branches: `Booking` and `Address`. Both of them will require a `Member`, and the functionality will reuse the same `Member` for both of them.

```csharp
_context.Seed(new AddressSeed(), new BookingSeed());
```

## Seed your most complicated data separately

It is most performant to seed everything in a single save. However, for complex data setups it's recommended to split out the seeding in stages to improve readability.
Sometimes you need complicated seed data, and can't work out how to set it all up using the `EntitySeed` classes. In some scenarios, this may be made easier by seeding the data in two stages.

If a specific test in the class is more complex than others, consider seeding the data for that test from within the test, rather than the constructor:

```csharp
public class MyTests
{
    public MyTests() 
    {
        // Seed simple data here that all tests need
    }
    
    [Fact]
    public void MySimpleUnitTests()
    {
        // No addititional setup required        
    }
    
    [Fact]
    public void MyComplexUnitTests()
    {
        // Additional commplex seeding goes here        
    }
}


```
