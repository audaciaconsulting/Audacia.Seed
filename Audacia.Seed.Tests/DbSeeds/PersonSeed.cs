using System.Collections.Generic;
using Audacia.Random.Extensions;
using Audacia.Seed.Tests.Entities;

namespace Audacia.Seed.Tests.DbSeeds
{
	public class PersonSeed : DbSeed<Person>, IDependsOn<Job>
	{
		public override Person Single()
		{
			var jobCount = Random.Next(1, 4);
			var jobs = Existing<Job>().TakeRandom(jobCount); // todo: TakeRandom(int min, int max)
			var person = new Person { Name = Random.Surname() };

			foreach (var job in jobs)
				person.Jobs.Add(job);

			return person;
		}
	}

	public class JobSeed : DbSeed<Job>
	{
		public override IEnumerable<Job> Defaults()
		{
			yield return new Job {Name = "Cleaner"};
			yield return new Job {Name = "Software Engineer"};
			yield return new Job {Name = "Director"};
		}
	}
}