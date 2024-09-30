using System.Linq.Expressions;
using Audacia.Core.Extensions;
using Audacia.Seed.Options;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Entities.Enums;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class FacilityTypeEntitySeed : EntitySeed<FacilityTypeEntity>
{
    private readonly List<FacilityType> _facilityTypes = Enum.GetValues<FacilityType>().ToList();

    public FacilityTypeEntitySeed()
    {
        Options.InsertionBehavior = SeedingInsertionBehaviour.TryFindExisting;
    }

    public override Expression<Func<FacilityTypeEntity, bool>> Predicate(int index)
    {
        return fte => fte.Type == _facilityTypes.ElementAt(index);
    }

    protected override FacilityTypeEntity GetDefault(int index, FacilityTypeEntity? previous)
    {
        var facilityType = _facilityTypes.ElementAt(index);
        return new FacilityTypeEntity
        {
            Type = facilityType,
            Name = facilityType.ToString().SplitCamelCase(),
            Description = $"Description for {facilityType}"
        };
    }
}