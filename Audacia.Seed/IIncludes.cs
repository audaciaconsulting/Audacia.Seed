namespace Audacia.Seed
{
	/// <summary>Specifies that a <see cref="DbSeed"/> includes another entity as some dependant child type.</summary>
	/// <typeparam name="T">The type of entity that this seed includes.</typeparam>
	public interface IIncludes<T> { }
}