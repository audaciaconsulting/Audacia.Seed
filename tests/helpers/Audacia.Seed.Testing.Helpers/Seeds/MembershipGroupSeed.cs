using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

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