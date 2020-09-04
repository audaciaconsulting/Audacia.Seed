using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audacia.Seed.AspNetCoreIdentity.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity
{
    /// <summary>
    /// The base implementation of an IdentitySeed.
    /// </summary>
    /// <typeparam name="TApplicationUser">The <see cref="IdentityUser"/> class for your application.</typeparam>
    /// <typeparam name="TKey">The primary key of your <see cref="IdentityUser"/> class.</typeparam>
    public abstract class IdentitySeed<TApplicationUser, TKey> : DbSeed,
        IIdentitySeed<IdentitySeedModel<TApplicationUser, TKey>>
        where TApplicationUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
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

        public IEnumerable<IdentitySeedModel<TApplicationUser, TKey>> GetAll() => Enumerable
            .Range(0, Count)
            .Select(_ => GetSingle())
            .Where(x => x != null)
            .Concat(Defaults())
            .ToArray();

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        public override object SingleObject() => GetSingle();

        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        public override IEnumerable<object> DefaultObjects() => Defaults().ToList();

        /// <summary>
        /// Seeds new application users into the database, existing users will be skipped.
        /// </summary>
        /// <param name="userManager">Asp.NetCore UserManager</param>
        /// <param name="token">Cancellation token</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="ArgumentNullException">Occurs when <see cref="UserManager{TUser}"/> or <see cref="IdentitySeed{TApplicationUser,TKey}"/> is null.</exception>
        /// <exception cref="IdentityException">Occurs when the <see cref="UserManager{TUser}"/> was unable to create a user.</exception>
        public async Task ConfigureAsync(UserManager<TApplicationUser> userManager, CancellationToken token)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            token.ThrowIfCancellationRequested();

            // Generate all application users to seed.
            var identitySeeds = GetAll().ToArray();
            foreach (var identitySeed in identitySeeds)
            {
                token.ThrowIfCancellationRequested();

                // Check if user exists
                var userIdentifier = identitySeed.ApplicationUser.Email;
                var existingUser = await userManager.FindByEmailAsync(userIdentifier);
                if (existingUser == null)
                {
                    // Create a new user with password (if provided)
                    var createUserTask = string.IsNullOrWhiteSpace(identitySeed.Password)
                        ? userManager.CreateAsync(identitySeed.ApplicationUser)
                        : userManager.CreateAsync(identitySeed.ApplicationUser, identitySeed.Password);

                    var identityResult = await createUserTask;
                    if (!identityResult.Succeeded)
                    {
                        throw new IdentityException(identityResult.Errors, $"Unable to create user. {userIdentifier}");
                    }
                }
            }
        }
    }
}