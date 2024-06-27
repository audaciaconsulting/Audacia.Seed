using System.Reflection;

namespace Audacia.Seed.Models;

/// <summary>
/// Gets model information for a provided entity.
/// </summary>
public record EntityModelInformation
{
    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    public required Type EntityType { get; set; }

    /// <summary>
    /// Gets or sets the property info(s) that make up the primary key.
    /// </summary>
    internal List<PropertyInfo>? PrimaryKey { get; set; }

    /// <summary>
    /// Gets or sets the list of required navigation properties for the entity.
    /// </summary>
    internal List<NavigationPropertyConfiguration> RequiredNavigationProperties { get; set; } = [];
}