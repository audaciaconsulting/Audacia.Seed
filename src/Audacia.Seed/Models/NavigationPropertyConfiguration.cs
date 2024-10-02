using System.Reflection;

namespace Audacia.Seed.Models;

/// <summary>
/// Information for a single navigation property.
/// </summary>
/// <param name="NavigationProperty">Property info for the navigation property.</param>
/// <param name="ForeignKeyProperty">Property info for the corresponding FK, if exists.</param>
public record NavigationPropertyConfiguration(PropertyInfo NavigationProperty, PropertyInfo? ForeignKeyProperty);