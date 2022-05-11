using System;
using System.Collections.Generic;
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
		protected DbSeed()
		{
			Dependencies = GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDependsOn<>))
                .Select(i => i.GenericTypeArguments.Single())
				.ToList();

			IncludedTypes = GetType()
                .GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIncludes<>))
                .Select(i => i.GenericTypeArguments.Single())
				.Concat(new[] { EntityType })
				.Distinct()
				.ToList();
		}

		/// <summary>Gets the number of entities to be seeded. This can be overridden in a subclass or set in the config file.</summary>
		public virtual int Count { get; internal set; }

        /// <summary>Gets a <see cref="System.Random"/> instance for generating random property values.</summary>
        protected Random Random { get; } = new Random();

        /// <summary>Gets the type of entity this seed class generates.</summary>
        public abstract Type EntityType { get; }

        /// <summary>This returns a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance an entity.</returns>
        public abstract object SingleObject();

        /// <summary>This returns all instances of the entity's defaults.</summary>
        /// <returns>All instances of an entities default objects.</returns>
        public abstract IEnumerable<object> DefaultObjects();

        /// <summary>This method returns multiple instances of the entity to be seeded, the number of which is specified by the <see cref="Count"/> property.</summary>
        /// <returns>Multiple instances of an entity.</returns>
        public IEnumerable<object> AllObjects() => Enumerable
			.Range(0, Count)
			.Select(_ => SingleObject())
			.Where(x => x != null)
			.Concat(DefaultObjects());

		/// <summary>
		/// Gets the seed context for the seed.
		/// </summary>
		internal SeedContext SeedContext { get; private set; } = new SeedContext();

		/// <summary>Returns an instance of each of the exported <see cref="DbSeed"/> types from the specified assembly.</summary>
		/// <exception cref="ArgumentNullException"><paramref name="assembly"/> is <see langword="null"/>.</exception>
		/// <param name="assembly">The <see cref="Assembly"/> from which the seeds will be generated.</param>
		/// <returns>An instance of <see cref="DbSeed"/>.</returns>
		public static IEnumerable<DbSeed> FromAssembly(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			var context = new SeedContext();
			var seeds = assembly.GetExportedTypes()
                .Where(t => typeof(DbSeed).IsAssignableFrom(t) && t.GetInterfaces()
                    .Where(i => i.IsGenericType)
                    .All(i => i.GetGenericTypeDefinition() != typeof(IIdentitySeed<>)))
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
		/// <exception cref="InvalidDataException">Occurs when cyclic dependencies are detected in the list of <see cref="DbSeed"/>.</exception>
		/// <param name="source">The unsorted <see cref="IEnumerable{DbSeed}"/>.</param>
		/// <returns>The topologically sorted <see cref="IEnumerable{DbSeed}"/>.</returns>
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
					throw new InvalidDataException("Cyclic dependencies detected in the following seed fixtures: " + string.Join(", ", list.Select(x => x.EntityType.Name)));
				}

				list.RemoveAll(s => removed.Contains(s));
			}
		}

		/// <summary>Gets dependencies.</summary>
		internal ICollection<Type> Dependencies { get; }

		/// <summary>Gets included types.</summary>
		internal ICollection<Type> IncludedTypes { get; }
	}

	/// <summary>The base class for a database seed fixture.</summary>
	/// <typeparam name="T">The type of entity this seed class generates.</typeparam>
	public abstract class DbSeed<T> : DbSeed, IDbSeed<T> where T : class
	{
		/// <summary>This method should return entities instances which should be seeded by default.</summary>
		/// <returns>Entities instances.</returns>
		public virtual IEnumerable<T> Defaults() => Enumerable.Empty<T>();

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance of an entity.</returns>
        public virtual T GetSingle() => default;

        /// <summary>This property can be used by derived types to check what data has already been seeded.</summary>
        /// <typeparam name="TEntity">The type of entities to return.</typeparam>
        /// <returns>Multiple seeded entities.</returns>
        protected IEnumerable<TEntity> ExistingEntities<TEntity>() where TEntity : class =>
			SeedContext.Entries<TEntity>();

		/// <summary>
		/// This method can be used to search for a specific DbSeed instance amongst the data that has already been seeded.
		/// </summary>
		/// <typeparam name="TEntity">The type of entity to return.</typeparam>
		/// <param name="selectorFunc">Function providing criteria for selecting the entity.</param>
		/// <returns>The first entity found matching the criteria in the selector function, or null if none found.</returns>
		protected TEntity ExistingEntity<TEntity>(Func<TEntity, bool> selectorFunc) where TEntity : class =>
			SeedContext.Entries<TEntity>().FirstOrDefault(selectorFunc);

		/// <summary>Gets or sets the previous entity of this type to be seeded, or null if the current is the first.</summary>
		protected T Previous { get; set; }

		/// <summary>Gets the type of entity this seed class generates.</summary>
		public override Type EntityType => typeof(T);

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance of the entity to be seeded.</returns>
        public override object SingleObject()
		{
			var result = GetSingle();
			Previous = result;

			SeedContext.Add(result, EntityType);
			return result;
		}

        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        /// <returns>Entity instances which should be seeded by default.</returns>
        public override IEnumerable<object> DefaultObjects()
		{
			var results = Defaults().ToList();

			foreach (var result in results)
			{
				SeedContext.Add(result, EntityType);
			}

			return results;
		}
	}
}