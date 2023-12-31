using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Audacia.Seed.TestFixtures.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
    /// <summary>
    /// Test database context class.
    /// </summary>
    /// 
    public class TestDbContext : DbContext
	{
        /// <summary>
        /// Constructor for <see cref="TestDbContext"/>.
        /// </summary>
		public TestDbContext() : base(Options) { }

        /// <summary>
        /// Gets or sets <see cref="DbSet{Person}"/>
        /// </summary>
        public DbSet<Person> People { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="DbSet{Holiday}"/>
        /// </summary>
		public DbSet<Holiday> Holidays { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="DbSet{Job}"/>
        /// </summary>
		public DbSet<Job> Jobs { get; set; } = null!;

        /// <summary>
        /// Gets or sets <see cref="DbSet{Location}"/>
        /// </summary>
		public DbSet<Location> Locations { get; set; } = null!;

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