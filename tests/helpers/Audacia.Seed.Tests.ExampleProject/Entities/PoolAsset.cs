using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class PoolAsset : Asset
{
    [SetsRequiredMembers]
    public PoolAsset(string name) : base(name)
    {
    }

    public int PoolId { get; set; }

    public Pool Pool { get; set; } = null!;
}