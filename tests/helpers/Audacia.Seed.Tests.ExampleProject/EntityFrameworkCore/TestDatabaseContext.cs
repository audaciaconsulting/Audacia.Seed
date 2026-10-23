using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore;

public class TestDatabaseContext(DbContextOptions<TestDatabaseContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetAssembly(typeof(TestDatabaseContext))!);
        base.OnModelCreating(modelBuilder);
    }
}