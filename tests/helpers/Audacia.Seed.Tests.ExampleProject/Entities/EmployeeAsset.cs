using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

[method: SetsRequiredMembers]
public class EmployeeAsset(string name) : Asset(name)
{
    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;
}