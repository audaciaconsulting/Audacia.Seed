using System.Reflection;

namespace Audacia.Seed;

/// <summary>
/// Something that will be seeded before a <see cref="IEntitySeed"/>.
/// </summary>
public interface ISeedPrerequisite
{
    /// <summary>
    /// Gets a seed for this prerequisite.
    /// </summary>
    /// <returns>A seed for the prerequisite.</returns>
    IEntitySeed Seed { get; }

    /// <summary>
    /// Gets the type of the prerequisite being seeded.
    /// </summary>
    Type EntityType { get; }

    /// <summary>
    /// Gets the property that this prerequisite is for.
    /// </summary>
    PropertyInfo PropertyInfo { get; }
}