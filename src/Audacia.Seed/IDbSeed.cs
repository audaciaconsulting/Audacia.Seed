using System;
using System.Collections.Generic;

namespace Audacia.Seed
{
    /// <summary>
    /// A fixture that will provide a number of entities for database seeding.
    /// </summary>
    public interface IDbSeed
    {
        /// <summary>Gets the number of entities to be seeded. This can be overridden in a subclass or set in the config file.</summary>
        int Count { get; }

        /// <summary>Gets the type of entity this seed class generates.</summary>
        Type EntityType { get; }

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance of an entity.</returns>
        object SingleObject();

        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        /// <returns>Entity instances which shouldbe seeded by default.</returns>
        IEnumerable<object> DefaultObjects();

        /// <summary>This method returns multiple instances of the entity to be seeded, the number of which is specified by the <see cref="DbSeed.Count"/> property.</summary>
        /// <returns>Multiple instances of the entity to be seeded.</returns>
        IEnumerable<object> AllObjects();
    }

    /// <summary>
    /// A typed fixture that will provide a number of entities for database seeding.
    /// </summary>
    /// <typeparam name="TEntity">The database entity to seed.</typeparam>
    public interface IDbSeed<out TEntity> : IDbSeed
        where TEntity : class
    {
        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        /// <returns>Entities which should be seeded by default.</returns>
        IEnumerable<TEntity> Defaults();

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single entity.</returns>
        TEntity GetSingle();
    }
}