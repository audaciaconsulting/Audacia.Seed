using Audacia.Seed.Extensions;
using Audacia.Seed.Tests.Entities;
using AutoFixture;
using Xunit;

namespace Audacia.Seed.Tests
{
	public class UnitTest
	{
		[Fact]
		public void TestDbSeed()
		{
			var fixture = new Fixture();
			var assembly = GetType().Assembly;
			fixture.RegisterSeeds(assembly);

			var person = fixture.Create<Person>();
			Assert.Equal("Dave", person.Name); // everyone is dave
		}
	}
}