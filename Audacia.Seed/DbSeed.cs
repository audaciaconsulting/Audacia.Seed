using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;

namespace Audacia.Seed
{
	/// <summary>Specifies that a <see cref="DbSeed"/> requires another entity to have been seeded before its own entities.</summary>
	/// <typeparam name="T">The type of entity that this seed is dependant on.</typeparam>
	public interface IDependsOn<T> { }

	public abstract class DbSeed
	{
		public DbSeed() { }
		
		protected System.Random Random { get; } = new System.Random();
		
		public abstract Type EntityType { get; }

		internal abstract object SingleObject();
	}

	public abstract class DbSeed<T> : DbSeed where T : class
	{
		public virtual IEnumerable<T> Defaults() => Enumerable.Empty<T>();
		
		public virtual T Single() => default(T);

		protected internal ICollection<TEntity> Existing<TEntity>() where TEntity : class => throw new NotImplementedException("Implement it you dummy.");

		protected internal T Previous { get; internal set; }
		
		internal IDictionary<Type, object[]> SeedContext { get; set; }

		public override Type EntityType => typeof(T);

		internal override object SingleObject() => Single();
	}
}