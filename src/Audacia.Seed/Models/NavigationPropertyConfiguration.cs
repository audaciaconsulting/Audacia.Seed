using System.Reflection;

namespace Audacia.Seed.Models;

public record NavigationPropertyConfiguration(PropertyInfo NavigationProperty, PropertyInfo? ForeignKeyProperty);