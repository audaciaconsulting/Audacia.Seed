using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class OlympicPoolSeed : EntitySeed<Pool>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Pool, IEnumerable<Facility>, Facility>(
            pool => pool.Facilities,
            this,
            new FacilitySeed(),
            2),
    ];

    protected override Pool GetDefault(
        int index,
        Pool? previous)
    {
        return new Pool
        {
            Name = "50m Pool",
        };
    }
}