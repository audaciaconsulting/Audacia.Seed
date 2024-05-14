using Audacia.Seed.Customisation;
using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class FacilitySeed : EntitySeed<Facility>
{
    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedPrerequisite<Facility, Employee>(f => f.Owner),
        new SeedPrerequisite<Facility, Employee>(f => f.Manager),
        new SeedPrerequisite<Facility, FacilityTypeEntity>(f => f.TypeEntity)
    ];

    protected override Facility GetDefault(int index, Facility? previous)
    {
        return new Facility
        {
            Name = Guid.NewGuid().ToString()
        };
    }

    public EntitySeed<Facility> ForRoom()
    {
        return this.WithPrerequisite(f => f.Room);
    }

    public EntitySeed<Facility> ForPool()
    {
        return this.WithPrerequisite(f => f.Pool);
    }
}