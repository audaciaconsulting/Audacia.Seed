using Audacia.Seed.Properties;

namespace Audacia.Seed.Constants;

/// <summary>
/// Constants used for seeding.
/// </summary>
internal static class SeedingConstants
{
    /// <summary>
    /// The suffix we use to check for foreign key properties.
    /// </summary>
    public const string ForeignKeySuffix = "Id";

    /// <summary>
    /// The default value <see cref="ISeedCustomisation.Order"/>.
    /// </summary>
    public const int DefaultCustomisationOrder = 100;
}