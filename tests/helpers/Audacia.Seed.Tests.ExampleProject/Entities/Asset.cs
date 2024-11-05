using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Asset
{
    [SetsRequiredMembers]
    public Asset(string name)
    {
        Name = name;
    }

    public int Id { get; set; }

    public required string Name { get; set; }

    public int AssetTypeId { get; set; }

    public AssetType AssetType { get; set; } = null!;

    public ICollection<CompanyAsset> CompanyAssets { get; set; } = [];
}