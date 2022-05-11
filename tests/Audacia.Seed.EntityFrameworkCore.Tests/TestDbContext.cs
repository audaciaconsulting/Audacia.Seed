using System.Data.Common;
using Audacia.Seed.TestFixtures.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
    public class TestDbContext : DbContext
	{
		public TestDbContext() : base(Options) { }

		public DbSet<Person> People { get; set; }

		public DbSet<Holiday> Holidays { get; set; }

		public DbSet<Job> Jobs { get; set; }

		public DbSet<Location> Locations { get; set; }

		private static DbConnection GetConnection() => new SqliteConnection("DataSource=:memory:");
        
		private static DbContextOptions<TestDbContext> Options
        {
            get
            {
                using var databaseConnection = GetConnection();
                return new DbContextOptionsBuilder<TestDbContext>()
                    .UseSqlite(databaseConnection)
                    .Options;
            }
        }
    }
}