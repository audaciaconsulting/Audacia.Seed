namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Company
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public ICollection<CompanyAsset> Assets { get; set; } = [];
}