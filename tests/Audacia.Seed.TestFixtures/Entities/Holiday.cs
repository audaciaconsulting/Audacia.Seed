using System;

namespace Audacia.Seed.TestFixtures.Entities
{
	public class Holiday : IDependsOn<Person>
	{
		public int Id { get; set; }

		public DateTime Start { get; set; }

		public DateTime End { get; set; }

		public Person Person { get; set; }

		public int PersonId { get; set; }

		public string Notes { get; set; }
	}
}