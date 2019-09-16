using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed
{
	/// <summary>Specifies that a <see cref="DbSeed"/> includes another entity as some dependant child type.</summary>
	/// <typeparam name="T">The type of entity that this seed includes.</typeparam>
	[SuppressMessage("StyleCop", "CA1040")]
	public interface IIncludes<T> { }
}