using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class CompanySeed : EntitySeed<Company>
{
    protected override Company GetDefault(int index, Company? previous)
    {
        return new Company
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}