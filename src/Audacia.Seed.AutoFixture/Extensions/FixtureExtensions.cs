using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using AutoFixture;

namespace Audacia.Seed.AutoFixture.Extensions
{
	/// <summary>Extension methods for AutoFixture's <see cref="Fixture"/> type.</summary>
	public static class FixtureExtensions
	{
		/// <summary>Registers a <see cref="DbSeed"/> with an AutoFixture <see cref="Fixture"/> instance.</summary>
		/// <typeparam name="T">The type of entity to be seeded.</typeparam>
		/// <param name="fixture">The AutoFixture <see cref="Fixture"/> for the seed.</param>
		/// <param name="seed">The <see cref="DbSeed"/> to be registered.</param>
		public static void RegisterSeed<T>(this Fixture fixture, DbSeed<T> seed) where T : class
		{
			fixture.Register(() => (T)seed.SingleObject());
		}

		/// <summary>Registers a collection pf <see cref="DbSeed"/> instances with an AutoFixture <see cref="Fixture"/> instance.</summary>
		/// <param name="fixture">The AutoFixture <see cref="Fixture"/> for the seeds.</param>
		/// <param name="assembly">The <see cref="Assembly"/> defining the seeds to be added.</param>
		public static void RegisterSeeds(this Fixture fixture, Assembly assembly)
		{
			var seeds = DbSeed.FromAssembly(assembly).ToArray();
			
			fixture.RegisterSeeds(seeds);
		}

		/// <summary>Registers the <see cref="DbSeed"/> types in the specified assembly with an AutoFixture <see cref="Fixture"/> instance.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="fixture"/> is <see langword="null"/>.</exception>
		/// <param name="fixture">The AutoFixture <see cref="Fixture"/> for the seeds.</param>
		/// <param name="seeds">The collection of seeds to be added.</param>
		public static void RegisterSeeds(this Fixture fixture, IEnumerable<DbSeed> seeds)
		{
            if (fixture == null)
            {
                throw new ArgumentNullException(nameof(fixture));
            }

            if (seeds == null)
            {
                throw new ArgumentNullException(nameof(seeds));
            }

            var enumeratedSeeds = seeds.ToArray();
            SeedConfiguration.Configure(enumeratedSeeds);

            foreach (var seed in enumeratedSeeds)
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
					seed = types.dbSeed.GetMethod(nameof(DbSeed<object>.GetSingle), flags.instance),
					register = typeof(FixtureExtensions).GetMethod(nameof(Register), flags.@static)
						?.MakeGenericMethod(seed.EntityType)
				};

				Debug.Assert(methods.seed != null, "methods.seed != null");
				Debug.Assert(methods.register != null, "methods.register != null");

				var @delegate = Delegate.CreateDelegate(types.func, seed, methods.seed);
				methods.register?.Invoke(null, new object[] { fixture, @delegate });
			}
		}

		private static void Register<T>(IFixture fixture, Delegate @delegate)
		{
			fixture.Register((Func<T>)@delegate);
		}
	}
}
