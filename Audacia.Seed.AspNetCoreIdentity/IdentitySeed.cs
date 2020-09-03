using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity
{
    /// <summary>
    /// The base implementation of an IdentitySeed
    /// </summary>
    /// <typeparam name="TApplicationUser">The <see cref="IdentityUser"/> class for your application.</typeparam>
    /// <typeparam name="TKey">The primary key of your <see cref="IdentityUser"/> class.</typeparam>
    public abstract class IdentitySeed<TApplicationUser, TKey> : DbSeed, IDbSeed<IdentitySeedModel<TApplicationUser, TKey>>
        where TApplicationUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    // TODO JP: create an interface that can be placed in Audacia.Seed and not add further dependencies
    {
        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        public virtual IEnumerable<IdentitySeedModel<TApplicationUser, TKey>> Defaults()
        {
            return Enumerable.Empty<IdentitySeedModel<TApplicationUser, TKey>>();
        }

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        public virtual IdentitySeedModel<TApplicationUser, TKey> GetSingle() => default;

        /// <summary>The type of entity this seed class generates.</summary>
        public override Type EntityType => typeof(IdentitySeedModel<TApplicationUser, TKey>);

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        public override object SingleObject() => GetSingle();

        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        public override IEnumerable<object> DefaultObjects() => Defaults().ToList();
    }
}