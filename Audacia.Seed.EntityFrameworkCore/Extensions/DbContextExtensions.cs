using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Extensions
{
	/// <summary>Extension methods for <see cref="DbContext"/> instances.</summary>
	public static class DbContextExtensions
	{
		/// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
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
				dbContext.ConfigureSeed(seed);
			}
		}

		/// <summary>Configures the <see cref="DbContext"/> to ensure data is seeded at application startup.</summary>
		[SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
		public static void ConfigureSeed(this DbContext dbContext, DbSeed seed)
		{
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (seed == null)
            {
                throw new ArgumentNullException(nameof(seed));
            }

            var data = seed.AllObjects();
            dbContext.AddRange(data);
		}
	}
}