using System;
using Xunit;

namespace Audacia.Seed.EntityFramework6.Tests
{
	public class UnitTest
	{
		[Fact]
		public void Test()
		{
			var dbContext = new TestDbContext();

			Assert.NotEmpty(dbContext.Holidays);
			Assert.NotEmpty(dbContext.Jobs);
			Assert.NotEmpty(dbContext.People);
		}
	}
}