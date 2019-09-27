using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Audacia.Seed.EntityFrameworkCore.Extensions;
using Audacia.Seed.TestFixtures.DbSeeds;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Audacia.Seed.EntityFrameworkCore.Tests
{
	public class UnitTest
	{
		[Fact]
		[SuppressMessage("ReSharper", "SA1305", Justification = "That's not hungarian notation you dummy'")]
		public void Test()
		{
			using (var dbContext = new TestDbContext())
			{
				dbContext.Database.OpenConnection();
				dbContext.Database.EnsureCreated();

				var assembly = Assembly.GetAssembly(typeof(JobSeed));
				dbContext.ConfigureSeeds(assembly);
				dbContext.SaveChanges();

				Assert.NotEmpty(dbContext.Holidays);
				Assert.NotEmpty(dbContext.Jobs);
				Assert.NotEmpty(dbContext.People);
			}
		}
	}
}