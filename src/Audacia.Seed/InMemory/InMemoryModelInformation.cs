using System.Reflection;
using Audacia.Seed.Models;

namespace Audacia.Seed.InMemory;

/// <inheritdoc />
internal record InMemoryModelInformation : IEntityModelInformation
{
    /// <inheritdoc />
    public Type EntityType { get; set; } = null!;

    /// <inheritdoc />
    public List<PropertyInfo>? PrimaryKey { get; }

    /// <inheritdoc />
    public List<NavigationPropertyConfiguration> RequiredNavigationProperties { get; set; } = [];
}