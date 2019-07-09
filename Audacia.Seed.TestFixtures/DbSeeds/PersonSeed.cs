using Audacia.Random.Extensions;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
	public class PersonSeed : DbSeed<Person>, IDependsOn<Job>
	{
		protected override Person Single()
		{
			var jobCount = Random.Next(1, 4);
			var jobs = Existing<Job>().TakeRandom(jobCount); // todo: TakeRandom(int min, int max)
			var person = new Person { Name = "Dave" };

			foreach (var job in jobs)
				person.Jobs.Add(job);

			return person;
		}
	}
}