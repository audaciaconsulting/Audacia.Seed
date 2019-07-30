using System.Data.Entity;
using System.Reflection;

namespace Audacia.Seed.EntityFramework6.Extensions
{
	public static class DbContextExtensions
	{
		public static void ConfigureSeeds(this DbContext dbContext, Assembly assembly)
		{
			var seeds = DbSeed.FromAssembly(assembly);
			
			foreach(var seed in seeds)
			{
				var entities = seed.AllObjects();
				dbContext.Set(seed.EntityType).AddRange(entities);
			}
		}
	}
}