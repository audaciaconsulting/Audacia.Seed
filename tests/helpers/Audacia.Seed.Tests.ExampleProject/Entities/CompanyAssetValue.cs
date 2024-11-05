namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class CompanyAssetValue
{
    public int Id { get; set; }

    public int CompanyAssetId { get; set; }

    public CompanyAsset CompanyAsset { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}