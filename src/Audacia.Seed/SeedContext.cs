using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Seed
{
	/// <summary>
	/// The seed context to which seeds can be added and from which previously added seeds can be retrieved.
	/// </summary>
	internal class SeedContext
	{
		private readonly IDictionary<Type, HashSet<object>> _dictionary = new Dictionary<Type, HashSet<object>>();

        /// <summary>
        /// Gets the seed entries of a specified type.
        /// </summary>
        /// <typeparam name="T">The type of entity to retrieve seeds for.</typeparam>
        /// <returns>The existing seed entries of the given type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "AV1551:Method overload should call another overload", Justification = "Other overload is being called but with extension method.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Maintainability", "ACL1009:Method overload should call another overload", Justification = "Other overload is being called but with extension method.")]
        public IEnumerable<T> Entries<T>() => Entries(typeof(T)).Cast<T>();

		/// <summary>
		/// Gets the seed entries of the given type, as a generic <see cref="IEnumerable"/>.
		/// </summary>
		/// <param name="type">The type of entity.</param>
		/// <returns>An <see cref="IEnumerable"/> of object.</returns>
        public virtual IEnumerable Entries(Type type)
		{
			return _dictionary.TryGetValue(type, out var value)
				? value
				: _dictionary[type] = new HashSet<object>();
		}

		/// <summary>
		/// Adds a seed to the <see cref="SeedContext"/>.
		/// </summary>
		/// <typeparam name="T">The entity type of the seed to be added.</typeparam>
		/// <param name="entity">The seed object.</param>
		public void Add<T>(object entity)
		{
			Add(entity, typeof(T));
		}

		/// <summary>
		/// Adds a seed to the <see cref="SeedContext"/>.
		/// </summary>
		/// <param name="entity">The seed object.</param>
		/// <param name="type">The entity type of the seed to be added.</param>
		public virtual void Add(object entity, Type type)
		{
			if (_dictionary.TryGetValue(type, out var value))
			{
				value.Add(entity);
			}
			else
			{
				var hashset = new HashSet<object> { entity };
				_dictionary[type] = hashset;
			}
		}
	}
}