using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Testing.Helpers.Seeds;

public class EmployeeSeed : EntitySeed<Employee>
{
    protected override Employee GetDefault(int index, Employee? previous)
    {
        return new Employee
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString()
        };
    }
}