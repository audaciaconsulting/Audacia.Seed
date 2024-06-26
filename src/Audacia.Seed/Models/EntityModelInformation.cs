using System.Reflection;

namespace Audacia.Seed.Models;

/// <summary>
/// Gets model information for a provided entity
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
    public List<PropertyInfo>? PrimaryKey { get; set; }

    public List<NavigationPropertyConfiguration> RequiredNavigationProperties { get; set; } = [];
}