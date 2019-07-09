using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Audacia.Seed
{
	/// <summary>The base class for a database seed fixture.</summary>
	public abstract class DbSeed
	{
		[SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Good luck subclassing this bad boy when its constructor is private.")]
		internal DbSeed()
		{
			Dependencies = GetType()
				.GetInterfaces()
				.Where(i => i.IsGenericType)
				.Where(i => i.GetGenericTypeDefinition() == typeof(IDependsOn<>))
				.Select(i => i.GenericTypeArguments.Single())
				.ToList();
			
			IncludedTypes = GetType()
				.GetInterfaces()
				.Where(i => i.IsGenericType)
				.Where(i => i.GetGenericTypeDefinition() == typeof(IIncludes<>))
				.Select(i => i.GenericTypeArguments.Single())
				.Concat(new[]{ EntityType })
				.Distinct()
				.ToList();
		}

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

		internal SeedContext SeedContext { get; private set; } = new SeedContext();
		
		/// <summary>Returns an instance of each of the exported <see cref="DbSeed"/> types from the specified assembly.</summary>
		/// <param name="assembly"></param>
		public static IEnumerable<DbSeed> FromAssembly(Assembly assembly)
		{
			var context = new SeedContext();
			var types = assembly.GetExportedTypes()
				.Where(t => typeof(DbSeed).IsAssignableFrom(t))
				.Select(Activator.CreateInstance)
				.Select(seed => (DbSeed) seed)
				.ToList();

			foreach (var type in types)
				type.SeedContext = context;
			
			return TopologicalSort(types);
		}

		/// <summary>Sorts an enumerable of <see cref="DbSeed"/> topologically, so dependencies are seeded before their dependants.</summary>
		public static IEnumerable<DbSeed> TopologicalSort(IEnumerable<DbSeed> source)
		{
			var list = source.ToList();

			while (list.Any())
			{
				var removed = new List<DbSeed>();
				foreach (var seed in list)
				{
					// If the source contains any dependencies of this, skip over it, don't return it yet.
					if (seed.Dependencies.All(d => !list.SelectMany(x => x.IncludedTypes).Contains(d)))
					{
						removed.Add(seed);
						yield return seed;
					}
				}
				
				if (!removed.Any())
				{
					var cyclicDependencies = string.Join(", ", list.Select(x => x.EntityType.Name));
					throw new InvalidDataException("Cyclic dependencies detected in the following seed fixtures: " + cyclicDependencies);
				}
				
				foreach(var x in removed)
					list.Remove(x);
			}

		}

		internal ICollection<Type> Dependencies { get; }
		
		internal ICollection<Type> IncludedTypes { get; }
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