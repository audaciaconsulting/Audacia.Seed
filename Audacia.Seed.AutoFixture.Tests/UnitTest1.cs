using System;
using Audacia.Seed.AutoFixture.Extensions;
using Audacia.Seed.Tests.Entities;
using AutoFixture;
using Xunit;

namespace Audacia.Seed.AutoFixture.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Test1()
		{
			var fixture = new Fixture();
			var assembly = GetType().Assembly;
			fixture.RegisterSeeds(assembly);

			var person = fixture.Create<Person>();
			Assert.Equal("Dave", person.Name); // everyone is dave
		}
	}
}