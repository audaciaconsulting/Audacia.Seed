using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Extensions
{
	/// <summary>Extension methods for <see cref="DbContext"/> instances.</summary>
	public static class DbContextExtensions
	{
        /// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
        /// <param name="databaseContext">The <see cref="DbContext"/> to add the seeds to.</param>
        /// <param name="assembly">The <see cref="Assembly"/> containing the seeds to be configured.</param>
        /// <exception cref="ArgumentNullException"><paramref name="databaseContext"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		public static void ConfigureSeeds(this DbContext databaseContext, Assembly assembly)
		{
            if (databaseContext == null)
            {
                throw new ArgumentNullException(nameof(databaseContext));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var seeds = DbSeed.FromAssembly(assembly);

            foreach (var seed in seeds)
			{
				databaseContext.ConfigureSeed(seed);
			}
		}

        /// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
        /// <param name="databaseContext">The <see cref="DbContext"/> to add the seed to.</param>
        /// <param name="seed">The <see cref="DbSeed"/> to be configured.</param>
        /// <exception cref="ArgumentNullException"><paramref name="databaseContext"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="seed"/> is <see langword="null"/>.</exception>
		public static void ConfigureSeed(this DbContext databaseContext, DbSeed seed)
		{
            if (databaseContext == null)
            {
                throw new ArgumentNullException(nameof(databaseContext));
            }

            if (seed == null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            var seeds = new List<DbSeed>() { seed };
            SeedConfiguration.Configure(seeds);
            
            if (seed is ISetDbContext seedFromDatabase)
            {
                seedFromDatabase.SetDbContext(databaseContext);
            }
            
            var data = seed.AllObjects();
            databaseContext.AddRange(data);
        }
	}
}
