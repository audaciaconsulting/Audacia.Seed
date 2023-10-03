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
        /// <returns>Entity instances which should be seeded by default.</returns>
        public virtual IEnumerable<IdentitySeedModel<TApplicationUser, TKey>> Defaults()
        {
            return Enumerable.Empty<IdentitySeedModel<TApplicationUser, TKey>>();
        }

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance of the entity to be seeded.</returns>
        public virtual IdentitySeedModel<TApplicationUser, TKey>? GetSingle() => default;

        /// <summary>Gets the type of entity this seed class generates.</summary>
        public override Type EntityType => typeof(IdentitySeedModel<TApplicationUser, TKey>);

        /// <summary>
        /// This method should return a collection containing all the identity seeds to be seeded.
        /// </summary>
        /// <returns>A collection containing all the identity seeds.</returns>
        public IEnumerable<IdentitySeedModel<TApplicationUser, TKey>> GetAll()
        {
            var defaults = Defaults();

            return Enumerable
                .Range(0, Count)
                .Select(_ => GetSingle())
                .Where(x => x != null)
                .Concat(defaults)
                .ToArray()!;
        }

        /// <summary>This method should return a single instance of the entity to be seeded.</summary>
        /// <returns>A single instance of the entity to be seeded.</returns>
        public override object? SingleObject() => GetSingle();

        /// <summary>This method should return entities instances which should be seeded by default.</summary>
        /// <returns>A list of entities which should be seeded by default.</returns>
        public override IEnumerable<object> DefaultObjects() => Defaults().ToList();

        /// <summary>
        /// Seeds new application users into the database, existing users will be skipped.
        /// </summary>
        /// <param name="userManager">Asp.NetCore UserManager.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>An awaitable task.</returns>
        /// <exception cref="ArgumentNullException">Occurs when <see cref="UserManager{TUser}"/> or <see cref="IdentitySeed{TApplicationUser,TKey}"/> is null.</exception>
        /// <exception cref="IdentityException">Occurs when the <see cref="UserManager{TUser}"/> was unable to create a user.</exception>
        public async Task ConfigureAsync(UserManager<TApplicationUser> userManager, CancellationToken token)
        {
            if (userManager == null) throw new ArgumentNullException(nameof(userManager));

            token.ThrowIfCancellationRequested();

            // Generate all application users to seed and loop through.
            foreach (var identitySeed in GetAll().ToArray())
            {
                token.ThrowIfCancellationRequested();

                // Check if user exists
                var existingUser = await userManager.FindByEmailAsync(identitySeed.ApplicationUser.Email).ConfigureAwait(false);
                if (existingUser == null)
                {
                    // Create a new user with password (if provided)
                    await CreateNewUserAsync(identitySeed, userManager).ConfigureAwait(true);
                }

                foreach (var role in identitySeed.Roles)
                {
                    await AddToRoleIfNotAlreadyAsync(identitySeed, userManager, existingUser, role).ConfigureAwait(true);
                }
            }
        }

        private static async Task CreateNewUserAsync(IdentitySeedModel<TApplicationUser, TKey> identitySeed, UserManager<TApplicationUser> userManager)
        {
            var createUserTask = CreateUserAsync(userManager, identitySeed.ApplicationUser, identitySeed.Password);

            var identityResult = await createUserTask.ConfigureAwait(false);
            if (!identityResult.Succeeded)
            {
                throw new IdentityException(identityResult.Errors, $"Unable to create user. {identitySeed.ApplicationUser.Email}");
            }
        }

        private static Task<IdentityResult> CreateUserAsync(
            UserManager<TApplicationUser> userManager,
            TApplicationUser user,
            string? password)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return string.IsNullOrWhiteSpace(password)
                ? userManager.CreateAsync(user)
                : userManager.CreateAsync(user, password);
        }

        private static async Task AddToRoleIfNotAlreadyAsync(IdentitySeedModel<TApplicationUser, TKey> identitySeed, UserManager<TApplicationUser> userManager, TApplicationUser? existingUser, string role)
        {
            var isInRole = await userManager.IsInRoleAsync(
                        existingUser ?? identitySeed.ApplicationUser,
                        role).ConfigureAwait(false);

            if (!isInRole)
            {
                var addToRoleResult = await userManager.AddToRoleAsync(
                    existingUser ?? identitySeed.ApplicationUser,
                    role).ConfigureAwait(false);

                if (!addToRoleResult.Succeeded)
                {
                    throw new IdentityException(
                        addToRoleResult.Errors,
                        $"Unable to add user ({identitySeed.ApplicationUser.Email}) to role {role}.");
                }
            }
        }
    }
}