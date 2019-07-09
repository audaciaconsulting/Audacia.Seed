using Xunit;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
	public class UnitTest
	{
		protected TestDbContext DbContext { get; } = new TestDbContext();

		[Fact]
		public void Test()
		{
			
		}
	}
}