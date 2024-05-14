using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class BookingSeed : EntitySeed<Booking>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Booking, Facility>(f => f.Facility),
        new SeedPrerequisite<Booking, Member>(f => f.Member)
    ];

    protected override Booking GetDefault(int index, Booking? previous)
    {
        return new Booking
        {
            Notes = Guid.NewGuid().ToString()
        };
    }
}