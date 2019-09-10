using System;
using System.Data.Entity;
using System.Reflection;

namespace Audacia.Seed.EntityFramework6.Extensions
{
	public static class DbContextExtensions
	{
		public static void ConfigureSeeds(this DbContext dbContext, Assembly assembly)
		{
			if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
			
			var seeds = DbSeed.FromAssembly(assembly);
			
			foreach(var seed in seeds)
			{
				var entities = seed.AllObjects();
				dbContext.Set(seed.EntityType).AddRange(entities);
			}
		}
	}
}