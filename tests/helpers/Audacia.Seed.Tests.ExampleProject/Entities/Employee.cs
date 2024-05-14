namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Employee
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public ICollection<Facility> FacilitiesManaged { get; set; } = [];

    public ICollection<Facility> FacilitiesOwned { get; set; } = [];
}