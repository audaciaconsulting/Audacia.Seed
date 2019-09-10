using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Extensions
{
	public static class DbContextExtensions
	{
		public static void ConfigureSeeds(this DbContext dbContext, Assembly assembly)
		{
			if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
			if (assembly == null) throw new ArgumentNullException(nameof(assembly));
			
			var seeds = DbSeed.FromAssembly(assembly);

			foreach (var seed in seeds)
			{
				var data = seed.AllObjects();
				dbContext.AddRange(data);
			}
		}
	}
}