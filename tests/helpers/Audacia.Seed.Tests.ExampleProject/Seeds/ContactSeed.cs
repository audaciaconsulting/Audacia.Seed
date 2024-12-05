using Audacia.Seed.Customisation;
using Audacia.Seed.Tests.ExampleProject.Entities;
using Audacia.Seed.Tests.ExampleProject.Entities.Enums;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class ContactSeed : EntitySeed<Contact>
{
    protected override Contact GetDefault(
        int index,
        Contact? previous)
    {
        return new Contact()
        {
            FirstName = Guid.NewGuid().ToString(),
            LastName = Guid.NewGuid().ToString(),
            Type = ContactType.Primary,
            CreatedAt = DateTimeOffset.Now
        };
    }

    /// <summary>
    /// Configures the contact as the secondary contact type.
    /// </summary>
    public void Secondary()
    {
        this.With(contact => contact.Type, ContactType.Secondary);
    }
}