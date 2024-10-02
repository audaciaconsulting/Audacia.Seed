namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Room
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Facility> Facilities { get; set; } = [];

    public int RegionId { get; set; }

    public Region Region { get; set; } = null!;
}