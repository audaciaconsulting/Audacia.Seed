This set of libraries was built to help developers generate organic seed data for performance and unit testing. The following are descriptions of each library and what their purpose is.

## Audacia.Seed

The base library which contains the types needed to create seed fixtures.
An example seed fixture would look as follows:

```c#
public class HolidaySeed : DbSeed<Holiday>,  IDependsOn<Person>
{
    protected override Holiday Single()
    {
        var start = Previous == null
            ? Random.DateTime()
            : Random.DateTimeFrom(Previous.End);

        var end = Random.DateTimeFrom(start);
        
        return new Holiday
        {
            Start = start,
            End = end,
            Person = Existing<Person>().Random(),
            Notes = Random.Sentence()
        };
    }
}
```

Notice the usage of the `Previous` property to ensure that the holiday being generated is after the previous one.

`IDependsOn<T>` is an interface used to specify that this seed should run after the one which creates instances of `T`. In this case, `T` is the `Person` type.

Similarly, the `IIncludes<T>` interface can be used to specify that a seed fixture includes some dependant type.

## Audacia.Seed.AutoFixture

This library allows for seed fixtures to be used with the popular AutoFixture library.
A single seed can be configured as follows:

```c#
var fixture = new Fixture();
fixture.RegisterSeed<HolidaySeed>();
var person = fixture.Create<Holiday>();
```

And a whole assembly of seed fixtures can be registered like this:

```c#
var fixture = new Fixture();
var assembly = Assembly.GetAssembly(typeof(HolidaySeed));
fixture.RegisterSeeds(assembly);
var person = fixture.Create<Holiday>();
```

## Audacia.Seed.EntityFramework6

This library facilitates the registering of seed fixtures with Entity Framework 6. The recommended way to seed data is by subclassing one of the database initializers and overriding the `Seed` method;

```c#
public class TestDbInitializer : DropCreateDatabaseAlways<TestDbContext>
{
    protected override void Seed(TestDbContext context)
    {
        var assembly = Assembly.GetAssembly(typeof(JobSeed));
        context.ConfigureSeed(assembly);
        base.Seed(context);
    }
}
```

Some initializers do not support the seed method however, but the call to `ConfigureSeed` can be made elsewhere in that case.

## Audacia.Seed.EntityFrameworkCore

This library facilitates the registering of seed fixtures with Entity Framework Core, which is written to configure its seed data via the `ModelBuilder`:

```c#
public class TestDbContext : DbContext
{
    public TestDbContext() { }

    public DbSet<Person> People { get; set; }

    public DbSet<Holiday> Holidays { get; set; }

    public DbSet<Job> Jobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureSeeds(Assembly.GetAssembly(typeof(JobSeed)));
        base.OnModelCreating(modelBuilder);
    }
}
```