using Audacia.Random.Extensions;
using Audacia.Seed.Tests.Entities;

namespace Audacia.Seed.Tests.DbSeeds
{
	public class HolidaySeed : DbSeed<Holiday>,  IDependsOn<Person>
	{
		public override Holiday Single()
		{
			var start = Previous == null
				? Random.DateTime()
				: Random.DateTimeFrom(Previous.End);

			var end = Random.DateTimeFrom(start);
			
			return new Holiday
			{
				Start = start,
				End = end,
				Person = Existing<Person>().Random(),
				Notes = Random.Sentence()
			};
		}
	}
}