using Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.Tests.TestHelpers;

public static class TestDatabaseContextBuilder
{
    public static TestDatabaseContext CreateContext()
    {
        return CreateContext(TestDatabaseContextFixture.DefaultConnectionString);
    }

    public static TestDatabaseContext CreateContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<TestDatabaseContext>()
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging();
        return new(builder.Options);
    }
}