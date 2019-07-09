using System.Data.Entity;
using System.Reflection;
using Audacia.Seed.EntityFramework6.Extensions;
using Audacia.Seed.TestFixtures.DbSeeds;

namespace Audacia.Seed.EntityFramework6.Tests
{
	public class TestDbInitializer : DropCreateDatabaseAlways<TestDbContext>
	{
		protected override void Seed(TestDbContext context)
		{
			var assembly = Assembly.GetAssembly(typeof(JobSeed));
			context.ConfigureSeed(assembly);
			base.Seed(context);
		}
	}
}