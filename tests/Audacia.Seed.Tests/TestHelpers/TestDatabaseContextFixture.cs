using Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;

namespace Audacia.Seed.Tests.TestHelpers;

/// <summary>
///  Global setup for testing a <see cref="TestDatabaseContext"/> for testing a real SQL sb.
/// </summary>
public class TestDatabaseContextFixture
{
    public TestDatabaseContextFixture()
    {
        InitiateConnection();
    }

    /// <summary>
    /// e.g "Server=(localdb)\mssqllocaldb;Database=DatabaseFixture-Gleadell.Ultra.Bll.Tests;Trusted_Connection=True"
    /// </summary>
    internal const string DefaultConnectionString = @"Server=(localdb)\mssqllocaldb;Database=Audacia.Seed.Tests;Trusted_Connection=True";

    private static void InitiateConnection()
    {
        using var context = TestDatabaseContextBuilder.CreateContext(DefaultConnectionString);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}