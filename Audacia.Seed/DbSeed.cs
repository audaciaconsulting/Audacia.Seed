﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Audacia.Seed
{
	/// <summary>The base class for a database seed fixture.</summary>
	public abstract class DbSeed : IDbSeed
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="DbSeed"/> class.
        /// </summary>
		[SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "Good luck subclassing this bad boy when its constructor is private.")]
		protected DbSeed()
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
				.Concat(new[] { EntityType })
				.Distinct()
				.ToList();
		}

		/// <summary>The number of entities to be seeded. This can be overridden in a subclass or set in the config file.</summary>
		public virtual int Count { get; internal set; }

		/// <summary><see cref="System.Random"/> instance for generating random property values.</summary>
		protected Random Random { get; } = new Random();

		/// <summary>The type of entity this seed class generates.</summary>
		public abstract Type EntityType { get; }

		/// <summary>This returns a single instance of the entity to be seeded.</summary>
		public abstract object SingleObject();

		/// <summary>This returns all instances of the entity's defaults.</summary>
		public abstract IEnumerable<object> DefaultObjects();

		/// <summary>This method returns multiple instances of the entity to be seeded, the number of which is specified by the <see cref="Count"/> property.</summary>
		public IEnumerable<object> AllObjects() => Enumerable
			.Range(0, Count)
			.Select(_ => SingleObject())
			.Where(x => x != null)
			.Concat(DefaultObjects());

		internal SeedContext SeedContext { get; private set; } = new SeedContext();

		/// <summary>Returns an instance of each of the exported <see cref="DbSeed"/> types from the specified assembly.</summary>
		public static IEnumerable<DbSeed> FromAssembly(Assembly assembly)
		{
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var context = new SeedContext();
            var seeds = assembly.GetExportedTypes()
				.Where(t => typeof(DbSeed).IsAssignableFrom(t))
				.Where(t => !typeof(IIdentitySeed<>).IsAssignableFrom(t))
				.Select(Activator.CreateInstance)
				.Select(seed => (DbSeed)seed)
				.ToList();

            SeedConfiguration.Configure(seeds);

            foreach (var type in seeds)
            {
                type.SeedContext = context;
            }

            return TopologicalSort(seeds);
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

				foreach (var x in removed)
                {
                    list.Remove(x);
                }
			}
		}

		internal ICollection<Type> Dependencies { get; }

		internal ICollection<Type> IncludedTypes { get; }
	}

	/// <summary>The base class for a database seed fixture.</summary>
	/// <typeparam name="T">The type of entity this seed class generates.</typeparam>
	public abstract class DbSeed<T> : DbSeed, IDbSeed<T> where T : class
	{
		/// <summary>This method should return entities instances which should be seeded by default.</summary>
		public virtual IEnumerable<T> Defaults() => Enumerable.Empty<T>();

		/// <summary>This method should return a single instance of the entity to be seeded.</summary>
		public virtual T GetSingle() => default;

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
			var result = GetSingle();
			Previous = result;

			SeedContext.Add(EntityType, result);
			return result;
		}

		/// <summary>This method should return entities instances which should be seeded by default.</summary>
		public override IEnumerable<object> DefaultObjects()
		{
			var results = Defaults().ToList();

			foreach (var result in results)
            {
                SeedContext.Add(EntityType, result);
            }

			return results;
		}
	}
}