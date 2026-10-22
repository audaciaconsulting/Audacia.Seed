using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

[method: SetsRequiredMembers]
public class PoolAsset(string name) : Asset(name)
{
    public int PoolId { get; set; }

    public Pool Pool { get; set; } = null!;
}