using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class PoolSeed : EntitySeed<Pool>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedChildrenPrerequisite<Pool, Facility>(
            pool => pool.Facilities,
            new FacilitySeed(),
            2),
    ];

    protected override Pool GetDefault(int index, Pool? previous)
    {
        return new Pool
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}