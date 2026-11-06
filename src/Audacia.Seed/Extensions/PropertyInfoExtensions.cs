using System.Reflection;

namespace Audacia.Seed.Extensions;

/// <summary>
/// Extensions for identifying properties reflected through different types.
/// </summary>
internal static class PropertyInfoExtensions
{
    /// <summary>
    /// Determine whether two properties refer to the same declaration, including inherited properties.
    /// </summary>
    /// <param name="property">The first property.</param>
    /// <param name="other">The property to compare to.</param>
    /// <returns>Whether both properties refer to the same declaration.</returns>
    internal static bool IsSameProperty(this PropertyInfo property, PropertyInfo other)
    {
        return property.DeclaringType == other.DeclaringType && property.HasSameMetadataDefinitionAs(other);
    }
}
