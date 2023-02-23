using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Audacia.Seed.EntityFramework6.Extensions
{
	/// <summary>Extension methods for <see cref="DbContext"/> instances.</summary>
	public static class DbContextExtensions
	{
        /// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
        /// <param name="databaseContext">Database context.</param>
        /// <param name="assembly">Assembly.</param>
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

            var seeds = DbSeed.FromAssembly(assembly).ToArray();
            
            foreach (var seed in seeds)
			{
                if (seed is ISetDbContext seedFromDatabase)
                {
                    seedFromDatabase.SetDbContext(databaseContext);
                }

                var entities = seed.AllObjects();
                databaseContext.Set(seed.EntityType).AddRange(entities);
			}
		}
	}
}
