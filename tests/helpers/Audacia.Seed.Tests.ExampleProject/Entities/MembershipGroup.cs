namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class MembershipGroup
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int RegionId { get; set; }

    public Region Region { get; set; } = null!;

    public int? ParentId { get; set; }

    public MembershipGroup? Parent { get; set; }

    public ICollection<Member> Members { get; set; } = [];

    public ICollection<MembershipGroup> ChildGroups { get; set; } = [];
}