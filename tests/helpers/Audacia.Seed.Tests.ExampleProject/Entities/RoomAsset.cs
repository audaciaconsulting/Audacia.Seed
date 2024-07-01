using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class RoomAsset : Asset
{
    [SetsRequiredMembers]
    public RoomAsset(string name) : base(name)
    {
    }

    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;
}