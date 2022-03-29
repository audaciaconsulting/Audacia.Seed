using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.TestClasses
{
    [SuppressMessage("Naming", "AV1704:Identifier contains one or more digits in its name", Justification = "These classes need to be numbered because they are used in a sorting test.")]
    public class TestSeed2 : DbSeed<Test2>, IDependsOn<Test1> { }
}