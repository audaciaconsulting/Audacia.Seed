using System.Collections.Generic;

namespace Audacia.Seed
{
    /// <summary>
    /// A typed fixture that will provide a application users for database seeding.
    /// Identity seeds will be excluded from the default seeding logic as they require a UserManager instead of a DbContext.
    /// </summary>
    /// <typeparam name="TIdentity">Application User Type.</typeparam>
    public interface IIdentitySeed<out TIdentity> : IDbSeed<TIdentity>
        where TIdentity : class
    {
        /// <summary>
        /// Returns a types list of all identities to seed.
        /// </summary>
        IEnumerable<TIdentity> GetAll();
    }
}