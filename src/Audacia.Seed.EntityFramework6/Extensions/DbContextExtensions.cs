using System;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Audacia.Seed.EntityFramework6.Extensions
{
	/// <summary>Extension methods for <see cref="DbContext"/> instances.</summary>
	public static class DbContextExtensions
	{
        /// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
        /// <param name="dbContext">Database context.</param>
        /// <param name="assembly">Assembly.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dbContext"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
        [SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
		public static void ConfigureSeeds(this DbContext dbContext, Assembly assembly)
		{
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var seeds = DbSeed.FromAssembly(assembly);

            foreach (var seed in seeds)
			{
                if (seed is ISetDbContext seedFromDatabase)
                {
                    seedFromDatabase.SetDbContext(dbContext);
                }

                var entities = seed.AllObjects();
                dbContext.Set(seed.EntityType).AddRange(entities);
			}
		}
	}
}