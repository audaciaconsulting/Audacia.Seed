using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

[method: SetsRequiredMembers]
public class RoomAsset(string name) : Asset(name)
{
    public int RoomId { get; set; }

    public Room Room { get; set; } = null!;
}