using System.Reflection;

namespace Audacia.Seed.Models;

/// <summary>
/// Gets model information for a provided entity.
/// </summary>
public interface IEntityModelInformation
{
    /// <summary>
    /// Gets or sets the type of the entity.
    /// </summary>
    Type EntityType { get; set; }

    /// <summary>
    /// Gets the property info(s) that make up the primary key.
    /// </summary>
    public List<PropertyInfo>? PrimaryKey { get; }

    /// <summary>
    /// Gets or sets the list of required navigation properties for the entity.
    /// </summary>
    List<NavigationPropertyConfiguration> RequiredNavigationProperties { get; set; }
}