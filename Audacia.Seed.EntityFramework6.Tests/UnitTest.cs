using Xunit;

namespace Audacia.Seed.EntityFramework6.Tests
{
	public class UnitTest
	{
		[Fact]
		public void Test()
		{
			using (var db = new TestDbContext())
			{
				Assert.NotEmpty(db.Holidays);
				Assert.NotEmpty(db.Jobs);
				Assert.NotEmpty(db.People);
			}
		}
	}
}