using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed
{
	/// <summary>Specifies that a <see cref="DbSeed"/> requires another entity to have been seeded before its own entities.</summary>
	/// <typeparam name="T">The type of entity that this seed is dependant on.</typeparam>
	[SuppressMessage("StyleCop", "CA1040")]
	public interface IDependsOn<T> { }
}