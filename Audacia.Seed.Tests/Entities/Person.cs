using System;
using System.Collections.Generic;

namespace Audacia.Seed.Tests.Entities
{
	public class Person
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public DateTime Birthday { get; set; }
		
		public ICollection<Job> Jobs { get; } = new HashSet<Job>();
	}

	public class Job
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<Person> Holders { get; } = new HashSet<Person>();
	}
}