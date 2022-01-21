using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Audacia.Seed.Tests.TestClasses;
using Xunit;

namespace Audacia.Seed.Tests
{
	[SuppressMessage("CA1812", "ClassNeverInstantiated.Local", Justification = "I've got some test classes here that get instantiated dynamically.'")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "I've got some test classes here that get instantiated dynamically.'")]
	public static class DbSeedTests
	{
		public class TopologicalSort
		{
			[Fact]
			public void Correctly_sorts_seed_fixtures_by_their_dependencies()
			{
				var seeds = new DbSeed[]
				{
					new TestSeed3(),
					new TestSeed1(),
					new TestSeed2()
				};

				var sorted = DbSeed.TopologicalSort(seeds).ToList();

				Assert.IsType<TestSeed1>(sorted[0]);
				Assert.IsType<TestSeed2>(sorted[1]);
				Assert.IsType<TestSeed3>(sorted[2]);
			}

			[Fact]
			public void Throws_an_exception_when_cyclic_references_are_detected()
			{
				var seeds = new DbSeed[]
				{
					new TestSeed3(),
					new BadSeed(),
					new TestSeed1(),
					new TestSeed2()
				};

				Assert.Throws<InvalidDataException>(() => DbSeed.TopologicalSort(seeds).ToList());
			}
		}
	}
}