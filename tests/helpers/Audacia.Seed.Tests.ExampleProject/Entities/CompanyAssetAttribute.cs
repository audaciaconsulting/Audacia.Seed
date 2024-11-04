namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class CompanyAssetAttribute
{
    public int Id { get; set; }

    public int CompanyAssetId { get; set; }

    public CompanyAsset CompanyAsset { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}