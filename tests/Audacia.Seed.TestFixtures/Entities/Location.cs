using System;
using System.Collections.Generic;
using System.Text;

namespace Audacia.Seed.TestFixtures.Entities
{
    public class Location
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Person> People { get; } = new HashSet<Person>();
    }
}