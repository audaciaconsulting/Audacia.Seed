using Audacia.Seed.Tests.ExampleProject.Entities.Enums;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class FacilityTypeEntity
{
    public FacilityType Type { get; set; }

    public required string Name { get; set; }

    public string? Description { get; set; }

    public ICollection<Facility> Facilities { get; set; } = [];
}