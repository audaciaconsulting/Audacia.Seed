using System.Data.Common;
using System.Data.Entity;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.EntityFramework6.Tests
{
    public class TestDbContext : DbContext
	{
		public TestDbContext() : base(GetConnection(), true)
        {
            var testDbInitialiser = new TestDbInitializer();
			Database.SetInitializer(testDbInitialiser);
		}

		public DbSet<Person> People { get; set; } = null!;

		public DbSet<Holiday> Holidays { get; set; } = null!;

		public DbSet<Job> Jobs { get; set; } = null!;

		public DbSet<Location> Locations { get; set; } = null!;

		private static DbConnection GetConnection() => Effort.DbConnectionFactory.CreateTransient();
	}
}