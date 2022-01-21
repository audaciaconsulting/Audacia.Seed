using Audacia.Random.Extensions;
using Audacia.Seed.TestFixtures.Entities;

namespace Audacia.Seed.TestFixtures.DbSeeds
{
	public class HolidaySeed : DbSeed<Holiday>,  IDependsOn<Person>
	{
		public override int Count => 10;

		public override Holiday GetSingle()
		{
			var start = Previous == null
				? Random.DateTime()
				: Random.DateTimeFrom(Previous.End);

			var end = Random.DateTimeFrom(start);
			var person = Existing<Person>().Random();

			return new Holiday
			{
				Start = start,
				End = end,
				Person = person,
				Notes = Random.Sentence()
			};
		}
	}
}