using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore.Extensions
{
	public static class ModelBuilderExtensions
	{
		public static void ConfigureSeeds(this ModelBuilder builder, Assembly assembly)
		{
			var seeds = DbSeed.FromAssembly(assembly);

			foreach (var seed in seeds)
			{
				var data = seed.MultipleObjects(DbSeed.DefaultCount).ToArray();
				builder.Entity(seed.EntityType).HasData(data);
			}
		}
	}
}