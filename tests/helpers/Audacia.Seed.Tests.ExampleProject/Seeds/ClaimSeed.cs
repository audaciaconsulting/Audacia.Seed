using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class ClaimSeed : EntitySeed<Claim>
{
    private readonly int NumberOfBooksToSeed = 2;

    public override IEnumerable<ISeedPrerequisite> Prerequisites() =>
    [
        new SeedChildrenPrerequisite<Claim, Contact>(
            bookCategory => bookCategory.Contacts,
            new ContactSeed(),
            NumberOfBooksToSeed)
    ];

    public ClaimSeed()
    {
    }

    public ClaimSeed(int numberOfBooksToSeed)
    {
        NumberOfBooksToSeed = numberOfBooksToSeed;
    }

    protected override Claim GetDefault(
        int index,
        Claim? previous)
    {
        return new Claim()
        {
            CreatedAt = DateTimeOffset.Now
        };
    }
}