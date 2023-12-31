using System.Collections.Generic;

namespace Audacia.Seed.TestFixtures.Entities
{
	public class Job
	{
		public int Id { get; set; }

		public string? Name { get; set; }

		public ICollection<Person> Holders { get; } = new HashSet<Person>();
	}
}