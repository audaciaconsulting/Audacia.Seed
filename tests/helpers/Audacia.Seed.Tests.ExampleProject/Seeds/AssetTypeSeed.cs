using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class AssetTypeSeed : EntitySeed<AssetType>
{
    protected override AssetType GetDefault(int index, AssetType? previous)
    {
        return new AssetType
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}