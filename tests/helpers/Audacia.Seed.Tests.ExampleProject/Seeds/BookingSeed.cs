using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class BookingSeed : EntitySeed<Booking>
{
    protected override Booking GetDefault(int index, Booking? previous)
    {
        return new Booking
        {
            Notes = Guid.NewGuid().ToString()
        };
    }
}