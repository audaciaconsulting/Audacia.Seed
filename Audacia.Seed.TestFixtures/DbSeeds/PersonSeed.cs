using System.Linq;
using Audacia.Random.Extensions;
using Audacia.Seed.EntityFrameworkCore;
using Audacia.Seed.TestFixtures.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
    public class PersonSeed : DbSeed<Person>, IDependsOn<Job>, IDependsOn<Location>, ISeedFromDatabase
    {
        public override int Count => 10;

        public DbContext DbContext { get; set; }

        public override Person GetSingle()
        {
            var jobCount = Random.Next(1, 4);
            var jobs = Existing<Job>().TakeRandom(jobCount);
            var person = new Person { Name = "Dave" };

            // Test accessing pre-existing data in DbContext
            Location locationInDbContext = null;
            if (DbContext != null)
            {
                locationInDbContext = DbContext.Set<Location>().FirstOrDefault(l => l.Name == "Leeds");
            }

            if (locationInDbContext != null)
            {
                person.Location = locationInDbContext;
            }
            else
            {
                // Test selecting for specific DbSeed instance within existing seeds
                person.Location = Existing<Location>(l => l.Name == "Bradford");
            }

            foreach (var job in jobs)
            {
                person.Jobs.Add(job);
            }

            return person;
        }
    }
}