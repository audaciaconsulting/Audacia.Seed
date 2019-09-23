using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Audacia.Seed
{
	internal class SeedContext
	{
		private readonly IDictionary<Type, HashSet<object>> _dictionary = new Dictionary<Type, HashSet<object>>();

		public IEnumerable<T> Entries<T>()
		{
			return Entries(typeof(T)).Cast<T>();
		}

		public IEnumerable Entries(Type type)
		{
			return _dictionary.TryGetValue(type, out var value)
				? value
				: _dictionary[type] = new HashSet<object>();
		}

		public void Add<T>(object entity)
		{
			Add(typeof(T), entity);
		}

		public void Add(Type type, object entity)
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