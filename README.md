This set of libraries was built to help developers generate organic seed data for performance and unit testing. The following are descriptions of each library and what their purpose is.

## Audacia.Seed

The base library which contains the types needed to create seed fixtures.
An example seed fixture would look as follows:

```csharp
public class HolidaySeed : DbSeed<Holiday>,  IDependsOn<Person>
{
    protected override Holiday GetSingle()
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

The `Existing` method allows seeds that have already been added to the `SeedContext` to be accessed. Passing no parameters returns all seeds of that type, whilst another overload allows for a specific seed to be returned.

As well as generating a randomised seed, it is also possible to create seeds based on static data using the by overriding the `Defaults()` method instead of `GetSingle()`. See example below:

```csharp
	public class JobSeed : DbSeed<Job>
	{
		public override IEnumerable<Job> Defaults()
		{
			yield return new Job { Name = "Cleaner" };
			yield return new Job { Name = "Software Engineer" };
			yield return new Job { Name = "Director" };
		}
	}
```

Seeds can be made to get common entities from the database context for cases where it is desirable to access pre-existing data. To do this a seed should inherit from `SeedFromDatabase`, You can use `DbEntity<TEntity>()` to query tables, and `DbView<TEntity>()` to query database views, failing that the database context is accessible as a protected member. An example of this is shown below:

```csharp
    public class PersonSeed : SeedFromDatabase<Person>, IDependsOn<Job>, IDependsOn<Location>
    {
        public override Person GetSingle()
        {
            return new Person
            {
                Name = Random.Forename(),
                Location = DbEntity<Location>(l => l.Name == "Leeds")
            };
        }
    }
```

###Configuration

There are two ways to configure seeding behaviour. All `DbSeed` classes have an overridable `Count` property which can be used to set the number of entities a given seed fixture should produce.

Additionally, the more recommended approach would be to use the application settings in web.config or equivalent. Settings can be specified either with or without the namespace:

```xml
<configuration>
    <appSettings>
        <add key="seed:Holiday" value="10" />
        <add key="seed:Job" value="10" />
        <add key="seed:Person" value="100" />
    </appSettings>
</configuration>
```

## Audacia.Seed.AutoFixture

This library allows for seed fixtures to be used with the popular AutoFixture library.
A single seed can be configured as follows:

```csharp
var fixture = new Fixture();
fixture.RegisterSeed<HolidaySeed>();
var person = fixture.Create<Holiday>();
```

And a whole assembly of seed fixtures can be registered like this:

```csharp
var fixture = new Fixture();
var assembly = Assembly.GetAssembly(typeof(HolidaySeed));
fixture.RegisterSeeds(assembly);
var person = fixture.Create<Holiday>();
```

## Audacia.Seed.EntityFramework6

This library facilitates the registering of seed fixtures with Entity Framework 6. The recommended way to seed data is by subclassing one of the database initializers and overriding the `Seed` method;

```csharp
public class TestDbInitializer : DropCreateDatabaseAlways<TestDbContext>
{
    protected override void Seed(TestDbContext context)
    {
        var assembly = Assembly.GetAssembly(typeof(JobSeed));
        context.ConfigureSeeds(assembly);
        base.Seed(context);
    }
}
```

Some initializers do not support the seed method however, but the call to `ConfigureSeed` can be made elsewhere in that case.

## Audacia.Seed.EntityFrameworkCore

This library facilitates the registering of seed fixtures with Entity Framework Core. Ef Core includes functionality to configure its seed data via the `ModelBuilder`, however, its not appropriate for randomly-generated seed data. The correct way to ensure seed data is added is to include the following in your application startup:

```csharp
var dbContext = new TestDbContext();
dbContext.Database.EnsureCreated();

var assembly = Assembly.GetAssembly(typeof(JobSeed));
dbContext.ConfigureSeeds(assembly);
dbContext.SaveChanges();
```