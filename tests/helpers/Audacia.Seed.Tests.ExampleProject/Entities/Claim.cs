namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Claim
{
    public int Id { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
}