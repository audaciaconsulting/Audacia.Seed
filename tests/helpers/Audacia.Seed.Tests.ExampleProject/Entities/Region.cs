namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Region
{
    public int Id { get; set; }

    public ICollection<Room> Rooms { get; set; } = [];

    public ICollection<MembershipGroup> MembershipGroups { get; set; } = [];
}