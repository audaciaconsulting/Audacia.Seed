using Audacia.Seed.Tests.ExampleProject.Entities.Enums;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Contact
{
    public int Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required ContactType Type { get; set; }

    public required DateTimeOffset CreatedAt { get; set; }

    public int ClaimId { get; set; }
}