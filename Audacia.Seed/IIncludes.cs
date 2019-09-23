using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed
{
	/// <summary>Specifies that a <see cref="DbSeed"/> includes another entity as some dependant child type.</summary>
	/// <typeparam name="T">The type of entity that this seed includes.</typeparam>
	[SuppressMessage("ReSharper", "UnusedTypeParameter", Justification = "Marker interface")]
	public interface IIncludes<T> { }
}