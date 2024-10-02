using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class MembershipGroupSeed : EntitySeed<MembershipGroup>
{
    protected override MembershipGroup GetDefault(int index, MembershipGroup? previous)
    {
        return new MembershipGroup
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}