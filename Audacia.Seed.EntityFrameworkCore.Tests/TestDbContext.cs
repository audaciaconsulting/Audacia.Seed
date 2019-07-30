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

		private static DbConnection Connection => new SqliteConnection("DataSource=:memory:");

		private static readonly DbContextOptions<TestDbContext> Options = new DbContextOptionsBuilder<TestDbContext>()
			.UseSqlite(Connection)
			.Options;
	}
}