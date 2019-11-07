using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
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

		private static DbConnection GetConnection() => new SqliteConnection("DataSource=:memory:");

		[SuppressMessage("ReSharper", "IDISP004", Justification = "This is just how EF works, sorry.")]
		private static DbContextOptions<TestDbContext> Options => new DbContextOptionsBuilder<TestDbContext>()
			.UseSqlite(GetConnection())
			.Options;
	}
}