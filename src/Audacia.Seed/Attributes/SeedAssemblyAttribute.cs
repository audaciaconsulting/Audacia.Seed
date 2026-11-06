namespace Audacia.Seed.Attributes;

/// <summary>
/// Used to tell the seeding logic where to find and load custom <see cref="EntitySeed{TEntity}"/>s.
/// </summary>
/// <remarks>
/// <para>Initializes a new instance of the <see cref="SeedAssemblyAttribute"/> class.</para>
/// </remarks>
/// <param name="name">The name of the assembly.</param>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public sealed class SeedAssemblyAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the name of the assembly.
    /// </summary>
    public string Name { get; } = name;
}