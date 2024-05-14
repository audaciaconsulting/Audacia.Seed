using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class MemberSeed : EntitySeed<Member>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Member, MembershipGroup>(m => m.MembershipGroup)
    ];

    protected override Member GetDefault(int index, Member? previous)
    {
        return new Member
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString()
        };
    }
}