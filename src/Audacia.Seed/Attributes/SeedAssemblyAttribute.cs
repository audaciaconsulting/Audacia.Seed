namespace Audacia.Seed.Attributes;

/// <summary>
/// Used to tell the seeding logic where to find and load custom <see cref="EntitySeed{TEntity}"/>s.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class SeedAssemblyAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the assembly.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedAssemblyAttribute"/> class.
    /// </summary>
    /// <param name="name">The name of the assembly.</param>
    public SeedAssemblyAttribute(string name)
    {
        Name = name;
    }
}