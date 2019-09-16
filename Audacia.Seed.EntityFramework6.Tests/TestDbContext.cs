using System.Data.Common;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.EntityFramework6.Tests
{
	public class TestDbContext : DbContext
	{
		[SuppressMessage("StyleCop", "CA2000")]
		public TestDbContext() : base(GetConnection(), true)
		{
			Database.SetInitializer(new TestDbInitializer());
		}

		public DbSet<Person> People { get; set; }

		public DbSet<Holiday> Holidays { get; set; }

		public DbSet<Job> Jobs { get; set; }

		private static DbConnection GetConnection() => Effort.DbConnectionFactory.CreateTransient();
	}
}