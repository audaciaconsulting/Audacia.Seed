namespace Audacia.Seed.Tests.TestClasses
{
	public class TestSeed2 : DbSeed<Test2>, IDependsOn<Test1> { }
}