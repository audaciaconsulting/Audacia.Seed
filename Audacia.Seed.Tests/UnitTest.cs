using System.IO;
using System.Linq;
using Xunit;

namespace Audacia.Seed.Tests
{
	public class DbSeedTests
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
			
			private class TestSeed1 : DbSeed<Test1> { }

			private class TestSeed2  : DbSeed<Test2>, IDependsOn<Test1> { }
			
			private class TestSeed3  : DbSeed<Test3>, IDependsOn<Test2> { }

			private class BadSeed : DbSeed<Test4>, IDependsOn<Test4> { }
			
			private class Test1 { }
			
			private class Test2 { }
			
			private class Test3 { }
			
			private class Test4 { }
		}
	
	}
}