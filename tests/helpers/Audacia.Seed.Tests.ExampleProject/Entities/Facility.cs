using Audacia.Seed.Tests.ExampleProject.Entities.Enums;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Facility
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public DateTime? ClosedDate { get; set; }

    public FacilityType Type { get; set; }

    public FacilityTypeEntity TypeEntity { get; set; } = null!;

    public int? RoomId { get; set; }

    public Room? Room { get; set; }

    public int? PoolId { get; set; }

    public Pool? Pool { get; set; }

    public int OwnerId { get; set; }

    public Employee Owner { get; set; } = null!;

    public int ManagerId { get; set; }

    public Employee Manager { get; set; } = null!;

    public ICollection<Booking> Bookings { get; set; } = [];
}