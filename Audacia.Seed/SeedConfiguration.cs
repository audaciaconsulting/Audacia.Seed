using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Audacia.Seed
{
	/// <summary>Contains configuration for a seed operation.</summary>
	public class SeedConfiguration
	{
		private const string Prefix = "seed:";

		private SeedConfiguration(int count) : this(count, count) { }
		
		private SeedConfiguration(int minimum, int maximum)
		{
			Minimum = minimum;
			Maximum = maximum;
		}

		/// <summary>The minimum number of entities to seed.</summary>
		public int Minimum { get; }

		/// <summary>The maximum number of entities to seed.</summary>
		public int Maximum { get; }

		/// <summary>Applies the seed settings in the application config file to the specified <see cref="DbSeed"/> instances, overwriting any hard-coded values.</summary>
		public static void Configure(IEnumerable<DbSeed> seeds)
		{
			if (seeds == null) throw new ArgumentNullException(nameof(seeds));
			var random = new Random();
			
			foreach (var seed in seeds)
			{
				var settings = ForType(seed.EntityType);
				seed.Count = random.Next(settings.Minimum, settings.Maximum + 1);
			}
		}
		
		/// <summary>Gets the settings specified for a given type.</summary>
		/// <exception cref="ConfigurationErrorsException">One or more values for seed configurations could not be parsed as an integer or a range.</exception>
		public static SeedConfiguration ForType(Type type)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			
			var key1 = Prefix + type.Name;
			var key2 = Prefix + type.FullName;
			
			var value = ConfigurationManager.AppSettings[key1]
				?? ConfigurationManager.AppSettings[key2];

			if (value == null)
				return new SeedConfiguration(0);
			
			// Is it just a number
			if (value.All(char.IsDigit))
				return new SeedConfiguration(int.Parse(value, NumberFormatInfo.InvariantInfo));
			
			// If its not a range then we can't handle this.

			var invalid = value.Count(x => x == '-') != 1
			              || value.Any(x => !char.IsDigit(x) || x != '-')
			              || value.First() == '-'
			              || value.Last() == '-';
			
			if (invalid) throw new ConfigurationErrorsException($"Can't parse configuration value \"{value}\"");

			var parts = value.Split('-');
			var min = int.Parse(parts[0], NumberFormatInfo.InvariantInfo);
			var max = int.Parse(parts[1], NumberFormatInfo.InvariantInfo);
			
			return new SeedConfiguration(min, max);
		}
	}
}