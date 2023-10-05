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
            var jobs = Existing<Job>().TakeRandom(jobCount);
            var person = new Person { Name = "Dave" };

            // Attempt to get data from DbContext, fallback to seeded entity
            var location = DbEntity<Location>(l => l.Name == "Leeds") ??
                           Existing<Location>(l => l.Name == "Bradford");

            if (location != null)
            {
                person.Location = location;
            }

            foreach (var job in jobs)
            {
                person.Jobs.Add(job);
            }

            return person;
        }
    }
}