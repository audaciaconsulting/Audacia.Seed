using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

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