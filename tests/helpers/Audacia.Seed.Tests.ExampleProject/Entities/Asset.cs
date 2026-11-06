using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

[method: SetsRequiredMembers]
public class Asset(string name)
{
    public int Id { get; set; }

    public required string Name { get; set; } = name;

    public int AssetTypeId { get; set; }

    public AssetType AssetType { get; set; } = null!;

    public ICollection<CompanyAsset> CompanyAssets { get; set; } = [];
}