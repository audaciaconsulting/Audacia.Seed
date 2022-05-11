using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.TestFixtures.DbSeeds;
using Audacia.Seed.TestFixtures.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
    public class DbContextExtensionsTests
    {
        [Fact]
        [SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
        public void ConfigureSeeds()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));
                dbContext.ConfigureSeeds(assembly);
                dbContext.SaveChanges();

                Assert.NotEmpty(dbContext.Holidays);
                Assert.NotEmpty(dbContext.Jobs);
                Assert.NotEmpty(dbContext.People);
                Assert.NotEmpty(dbContext.Locations);

                dbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        [SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
        public void ConfigureSeed()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();

                var seed = new JobSeed();
                dbContext.ConfigureSeed(seed);
                dbContext.SaveChanges();

                Assert.Empty(dbContext.Holidays);
                Assert.NotEmpty(dbContext.Jobs);
                Assert.Empty(dbContext.People);
                Assert.Empty(dbContext.Locations);

                dbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        [SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
        public void TestAddsSeedsToDbContext()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));

                dbContext.ConfigureSeeds(assembly);
                dbContext.SaveChanges();

                Assert.NotEmpty(dbContext.Holidays);
                Assert.NotEmpty(dbContext.Jobs);
                Assert.NotEmpty(dbContext.People);
                Assert.NotEmpty(dbContext.Locations);

                dbContext.Database.EnsureDeleted();
            }
        }

        [Fact]
        [SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
        public void TestAccessingPreExistingDataInDbContext()
        {
            using (var dbContext = new TestDbContext())
            {
                dbContext.Database.OpenConnection();
                dbContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));

                var location = new Location { Name = "Leeds" };
                dbContext.Locations.Add(location);
                dbContext.SaveChanges();

                dbContext.ConfigureSeeds(assembly);
                dbContext.SaveChanges();

                Assert.All(dbContext.People, p => Assert.True(p.Location.Name == "Leeds"));

                Assert.NotEmpty(dbContext.Holidays);
                Assert.NotEmpty(dbContext.Jobs);
                Assert.NotEmpty(dbContext.People);
                Assert.NotEmpty(dbContext.Locations);

                dbContext.Database.EnsureDeleted();
            }
        }
    }
}