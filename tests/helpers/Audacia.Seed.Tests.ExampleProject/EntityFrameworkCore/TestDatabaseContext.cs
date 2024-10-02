using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;

public class TestDatabaseContext : DbContext
{
    public TestDatabaseContext(DbContextOptions<TestDatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(TestDatabaseContext))!);
        base.OnModelCreating(modelBuilder);
    }
}