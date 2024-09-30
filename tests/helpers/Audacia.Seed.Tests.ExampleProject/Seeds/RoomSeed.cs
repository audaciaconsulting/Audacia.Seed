using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class RoomSeed : EntitySeed<Room>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Room, Region>(b => b.Region)
    ];

    protected override Room GetDefault(int index, Room? previous)
    {
        return new Room
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}