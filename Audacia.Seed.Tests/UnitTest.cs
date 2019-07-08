using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Audacia.Seed.Extensions;
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
		}
	}
}