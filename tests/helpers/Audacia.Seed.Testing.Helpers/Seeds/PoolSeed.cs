using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class PoolSeed : EntitySeed<Pool>
{
    protected override Pool GetDefault(int index, Pool? previous)
    {
        return new Pool
        {
            Name = Guid.NewGuid().ToString()
        };
    }
}