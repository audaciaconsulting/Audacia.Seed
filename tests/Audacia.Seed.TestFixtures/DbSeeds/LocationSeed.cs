using System;
using System.Collections.Generic;
using System.Text;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
    public class LocationSeed : DbSeed<Location>
    {
        public override IEnumerable<Location> Defaults()
        {
            yield return new Location { Name = "Bradford" };
            yield return new Location { Name = "Wakefield" };
        }
    }
}
