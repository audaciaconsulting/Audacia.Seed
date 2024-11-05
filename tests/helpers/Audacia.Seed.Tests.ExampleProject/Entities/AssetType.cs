namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class AssetType
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Asset> Assets { get; set; } = [];
}