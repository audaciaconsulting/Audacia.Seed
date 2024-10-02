using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class EmployeeAsset : Asset
{
    [SetsRequiredMembers]
    public EmployeeAsset(string name) : base(name)
    {
    }

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;
}