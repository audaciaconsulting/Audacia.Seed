using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
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

		/// <summary>Gets the minimum number of entities to seed.</summary>
		public int Minimum { get; }

		/// <summary>Gets the maximum number of entities to seed.</summary>
		public int Maximum { get; }

		/// <summary>
		/// Applies the seed settings in the application config file to the specified <see cref="DbSeed"/> instances, overwriting any hard-coded values.
		/// </summary>
		/// <param name="seeds">The <see cref="DbSeed"/> instances to configure.</param>
		/// <exception cref="ArgumentNullException"><paramref name="seeds"/> is <see langword="null"/>.</exception>
		public static void Configure(IEnumerable<DbSeed> seeds)
		{
            if (seeds == null)
            {
                throw new ArgumentNullException(nameof(seeds));
            }

            var random = new Random();

            foreach (var seed in seeds.Where(seed => !seed.Configured))
			{
				var settings = ForType(seed.EntityType);
				seed.Count = random.Next(settings.Minimum, settings.Maximum + 1);
				
				DbSeed.SetHasBeenConfigured(seed);
			}
		}

        /// <summary>
        /// Gets the settings specified for a given type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which to get the settings.</param>
        /// <returns>The <see cref="SeedConfiguration"/> for the given <paramref name="type"/>.</returns>
        /// <exception cref="ConfigurationErrorsException">One or more values for seed configurations could not be parsed as an integer or a range.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
        [SuppressMessage("Maintainability", "ACL1002:Member or local function contains too many statements", Justification = "Method is readable and maintainable.")]
        public static SeedConfiguration ForType(Type type)
		{
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var keyOne = Prefix + type.Name;
            var keyTwo = Prefix + type.FullName;

            var value = ConfigurationManager.AppSettings[keyOne]
                        ?? ConfigurationManager.AppSettings[keyTwo];

            if (value == null)
            {
                return new SeedConfiguration(0);
            }

			// Is it just a number
            if (value.All(char.IsDigit))
            {
                var count = int.Parse(value, NumberFormatInfo.InvariantInfo);
                return new SeedConfiguration(count);
            }

			// If its not a range then we can't handle this.
            var invalid = value.Count(x => x == '-') != 1
			              || value.Any(x => !char.IsDigit(x) || x != '-')
			              || value[0] == '-'
			              || value.Last() == '-';

            if (invalid)
            {
                throw new ConfigurationErrorsException($"Can't parse configuration value \"{value}\"");
            }

            var parts = value.Split('-');
            var min = int.Parse(parts[0], NumberFormatInfo.InvariantInfo);
            var max = int.Parse(parts[1], NumberFormatInfo.InvariantInfo);

            return new SeedConfiguration(min, max);
		}
	}
}
