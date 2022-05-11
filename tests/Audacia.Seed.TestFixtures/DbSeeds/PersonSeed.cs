using Audacia.Random.Extensions;
using Audacia.Seed.EntityFrameworkCore;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
    public class PersonSeed : SeedFromDatabase<Person>, IDependsOn<Job>, IDependsOn<Location>
    {
        public override int Count => 10;

        public override Person GetSingle()
        {
            var jobCount = Random.Next(1, 4);
            var jobs = ExistingEntities<Job>().TakeRandom(jobCount);
            var person = new Person { Name = "Dave" };

            // Attempt to get data from DbContext, fallback to seeded entity
            person.Location = DbEntity<Location>(l => l.Name == "Leeds") ??
                              ExistingEntity<Location>(l => l.Name == "Bradford");

            foreach (var job in jobs)
            {
                person.Jobs.Add(job);
            }

            return person;
        }
    }
}