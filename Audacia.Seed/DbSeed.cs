using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Audacia.Seed
{
	/// <summary>The base class for a database seed fixture.</summary>
	public abstract class DbSeed
	{
		internal DbSeed() { }

		// TODO: Remove this at some point.
		/// <summary>The number of entities to be seeded- temporary.</summary>
		public const int DefaultCount = 50;

		/// <summary><see cref="System.Random"/> instance for generating random property values.</summary>
		protected System.Random Random { get; } = new System.Random();

		/// <summary>The type of entity this seed class generates.</summary>
		public abstract Type EntityType { get; }

		/// <summary>This returns a single instance of the entity to be seeded.</summary>
		public abstract object SingleObject();

		/// <summary>This method returns multiple instances of the entity to be seeded.</summary>
		public IEnumerable<object> MultipleObjects(int count) => Enumerable.Range(0, count).Select(_ => SingleObject()).Where(x => x != null);

		/// <summary>Returns an instance of each of the exported <see cref="DbSeed"/> types from the specified assembly.</summary>
		/// <param name="assembly"></param>
		public static IEnumerable<DbSeed> FromAssembly(Assembly assembly)
		{
			return assembly.GetExportedTypes()
				.Where(t => typeof(DbSeed).IsAssignableFrom(t))
				.Select(Activator.CreateInstance)
				.Select(seed => (DbSeed) seed);
		}
	}

	/// <summary>The base class for a database seed fixture.</summary>
	/// <typeparam name="T">The type of entity this seed class generates.</typeparam>
	public abstract class DbSeed<T> : DbSeed where T : class
	{
		/// <summary>This method should return entities instances which should be seeded by default.</summary>
		protected virtual IEnumerable<T> Defaults() => Enumerable.Empty<T>();

		/// <summary>This method should return a single instance of the entity to be seeded.</summary>
		protected internal virtual T Single() => default(T);

		/// <summary>This property can be used by derived types to check what data has already been seeded.</summary>
		/// <typeparam name="TEntity">The type of entities to return.</typeparam>
		protected IEnumerable<TEntity> Existing<TEntity>() where TEntity : class => 
			SeedContext.Entries<TEntity>();

		/// <summary>This property references the previous entity of this type to be seeded, or null if the current is the first.</summary>
		protected T Previous { get; set; }

		private SeedContext SeedContext { get; } = new SeedContext();

		/// <summary>The type of entity this seed class generates.</summary>
		public override Type EntityType => typeof(T);

		/// <summary>This method should return a single instance of the entity to be seeded.</summary>
		public override object SingleObject()
		{
			var result = Single();
			Previous = result;
			
			SeedContext.Add(EntityType, result);
			return result;
		}

		/// <summary>This method should return entities instances which should be seeded by default.</summary>
		public IEnumerable<object> DefaultObjects()
		{
			var results = Defaults().ToList();
			Previous = results.First();

			foreach (var result in results)
				SeedContext.Add(EntityType, result);

			return results;
		}
	}
}