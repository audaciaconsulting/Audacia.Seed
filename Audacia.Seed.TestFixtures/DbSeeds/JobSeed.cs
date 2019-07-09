using System.Collections.Generic;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
	public class JobSeed : DbSeed<Job>
	{
		protected override IEnumerable<Job> Defaults()
		{
			yield return new Job {Name = "Cleaner"};
			yield return new Job {Name = "Software Engineer"};
			yield return new Job {Name = "Director"};
		}
	}
}