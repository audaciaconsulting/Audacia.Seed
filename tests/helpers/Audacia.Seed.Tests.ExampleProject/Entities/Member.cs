namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Member
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public MembershipLevel MembershipLevel { get; set; }

    public int MembershipGroupId { get; set; }

    public MembershipGroup MembershipGroup { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
}