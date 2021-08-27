using System.Reflection;
using Audacia.Seed.AutoFixture.Extensions;
using Audacia.Seed.TestFixtures.DbSeeds;
using Audacia.Seed.TestFixtures.Entities;
using AutoFixture;
using Xunit;

namespace Audacia.Seed.AutoFixture.Tests
{
	public class UnitTest
	{
        [Fact]
        public void Test()
		{
			var fixture = new Fixture();
			var assembly = Assembly.GetAssembly(typeof(JobSeed));
			fixture.RegisterSeeds(assembly);

			var person = fixture.Create<Person>();

			Assert.Equal("Dave", person.Name); // everyone is dave
		}
	}
}