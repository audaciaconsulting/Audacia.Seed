using System.Reflection;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.TestFixtures.DbSeeds;
using Audacia.Seed.TestFixtures.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
    /// <summary>
    /// Tests for DbContext extensions.
    /// </summary>
    public class DbContextExtensionsTests
    {
        /// <summary>
        /// Test for configuring seeds.
        /// </summary>
        [Fact]
        public void ConfigureSeeds()
        {
            using (var databaseContext = new TestDbContext())
            {
                databaseContext.Database.OpenConnection();
                databaseContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));
                databaseContext.ConfigureSeeds(assembly);
                databaseContext.SaveChanges();

                Assert.NotEmpty(databaseContext.Holidays);
                Assert.NotEmpty(databaseContext.Jobs);
                Assert.NotEmpty(databaseContext.People);
                Assert.NotEmpty(databaseContext.Locations);

                databaseContext.Database.EnsureDeleted();
            }
        }

        /// <summary>
        /// Test for configuring a seed.
        /// </summary>
        [Fact]
        public void ConfigureSeed()
        {
            using (var databaseContext = new TestDbContext())
            {
                databaseContext.Database.OpenConnection();
                databaseContext.Database.EnsureCreated();

                var seed = new JobSeed();
                databaseContext.ConfigureSeed(seed);
                databaseContext.SaveChanges();

                Assert.Empty(databaseContext.Holidays);
                Assert.NotEmpty(databaseContext.Jobs);
                Assert.Empty(databaseContext.People);
                Assert.Empty(databaseContext.Locations);

                databaseContext.Database.EnsureDeleted();
            }
        }

        /// <summary>
        /// Test for adding seeds to DbContext.
        /// </summary>
        [Fact]
        public void TestAddsSeedsToDbContext()
        {
            using (var databaseContext = new TestDbContext())
            {
                databaseContext.Database.OpenConnection();
                databaseContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));

                databaseContext.ConfigureSeeds(assembly);
                databaseContext.SaveChanges();

                Assert.NotEmpty(databaseContext.Holidays);
                Assert.NotEmpty(databaseContext.Jobs);
                Assert.NotEmpty(databaseContext.People);
                Assert.NotEmpty(databaseContext.Locations);

                databaseContext.Database.EnsureDeleted();
            }
        }

        /// <summary>
        /// Test for retrieving pre-existing seeds in DbContext.
        /// </summary>
        [Fact]
        public void TestAccessingPreExistingDataInDbContext()
        {
            using (var databaseContext = new TestDbContext())
            {
                databaseContext.Database.OpenConnection();
                databaseContext.Database.EnsureCreated();

                var assembly = Assembly.GetAssembly(typeof(JobSeed));

                var location = new Location { Name = "Leeds" };
                databaseContext.Locations.Add(location);
                databaseContext.SaveChanges();

                databaseContext.ConfigureSeeds(assembly);
                databaseContext.SaveChanges();

                Assert.All(databaseContext.People, p => Assert.True(p.Location.Name == "Leeds"));

                Assert.NotEmpty(databaseContext.Holidays);
                Assert.NotEmpty(databaseContext.Jobs);
                Assert.NotEmpty(databaseContext.People);
                Assert.NotEmpty(databaseContext.Locations);

                databaseContext.Database.EnsureDeleted();
            }
        }
    }
}