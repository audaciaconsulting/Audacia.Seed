using System.Reflection;
using Audacia.Seed.Models;

namespace Audacia.Seed.EntityFrameworkCore.Models;

/// <summary>
/// Gets model information for a provided entity.
/// </summary>
public record EntityFrameworkCoreModelInformation : IEntityModelInformation
{
    /// <inheritdoc />
    public required Type EntityType { get; set; }

    /// <inheritdoc />
    public List<PropertyInfo>? PrimaryKey { get; set; }

    /// <inheritdoc />
    public List<NavigationPropertyConfiguration> RequiredNavigationProperties { get; set; } = [];
}