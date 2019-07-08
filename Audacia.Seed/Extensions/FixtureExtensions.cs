using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using AutoFixture;

namespace Audacia.Seed.Extensions
{
	/// <summary>Extension methods for AutoFixture's <see cref="Fixture"/> type.</summary>
	public static class FixtureExtensions
	{
		/// <summary>Registers a <see cref="DbSeed"/> with an AutoFixture <see cref="Fixture"/> instance.</summary>
		public static void RegisterSeed<T>(this Fixture fixture, DbSeed<T> seed) where T : class
		{
			fixture.Register(seed.Single);
		}
		
		/// <summary>Registers a collection pf <see cref="DbSeed"/> instances with an AutoFixture <see cref="Fixture"/> instance.</summary>
		public static void RegisterSeeds(this Fixture fixture, Assembly assembly)
		{
			var seeds = assembly.GetExportedTypes()
				.Where(t => typeof(DbSeed).IsAssignableFrom(t))
				.Select(Activator.CreateInstance)
				.Select(seed => (DbSeed)seed);

			fixture.RegisterSeeds(seeds);
		}
		
		/// <summary>Registers the <see cref="DbSeed"/> types in the specified assembly with an AutoFixture <see cref="Fixture"/> instance.</summary>
		public static void RegisterSeeds(this Fixture fixture, IEnumerable<DbSeed> seeds)
		{
			foreach (var seed in seeds)
			{
				var flags = new
				{
					instance = BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy,
					@static = BindingFlags.NonPublic | BindingFlags.Static
				};

				var types = new
				{
					func = typeof(Func<>).MakeGenericType(seed.EntityType),
					dbSeed = typeof(DbSeed<>).MakeGenericType(seed.EntityType)
				};

				var methods = new
				{
					seed = types.dbSeed.GetMethod("Single", flags.instance),
					register = typeof(FixtureExtensions).GetMethod(nameof(Register), flags.@static)
						?.MakeGenericMethod(seed.EntityType)
				};
				
				Debug.Assert(methods.seed != null);
				Debug.Assert(methods.register != null);
				
				var @delegate = Delegate.CreateDelegate(types.func, seed, methods.seed);
				methods.register.Invoke(null, new object[] { fixture, @delegate });
			}
		}

		private static void Register<T>(IFixture fixture, Delegate @delegate)
		{
			fixture.Register((Func<T>)@delegate);
		}

		private static T Single<T>(DbSeed<T> seed) where T : class
		{
			return seed.Single();
		}
	}
}