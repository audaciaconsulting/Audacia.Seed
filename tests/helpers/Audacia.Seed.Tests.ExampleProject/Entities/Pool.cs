namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Pool
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Facility> Facilities { get; set; } = [];
}