namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class CompanyAsset
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public int AssetId { get; set; }

    public Asset Asset { get; set; } = null!;
}