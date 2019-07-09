using System.Data.Common;
using System.Data.Entity;
using Audacia.Seed.Tests.Entities;

namespace Audacia.Seed.EntityFramework6.Tests
{
	public class TestDbContext : DbContext
	{
		public TestDbContext() : base(Connection, false)
		{
			Database.SetInitializer(new TestDbInitializer());
		}

		public DbSet<Person> People { get; set; }

		public DbSet<Holiday> Holidays { get; set; }

		public DbSet<Job> Jobs { get; set; }

		private static DbConnection Connection => Effort.DbConnectionFactory.CreateTransient();
	}
}