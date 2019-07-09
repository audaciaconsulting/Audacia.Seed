using System.Data.Entity;
using System.Reflection;

namespace Audacia.Seed.EntityFramework6.Extensions
{
	public static class DbContextExtensions
	{
		public static void ConfigureSeed(this DbContext dbContext, Assembly assembly)
		{
			var seeds = DbSeed.FromAssembly(assembly);
			
			foreach(var seed in seeds)
			{
				var entities = seed.MultipleObjects(DbSeed.DefaultCount);
				dbContext.Set(seed.EntityType).AddRange(entities);
			}
		}
	}
}