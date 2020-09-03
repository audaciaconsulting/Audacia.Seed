using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Audacia.Seed.AspNetCoreIdentity.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity.Extensions
{
    public static class UserManagerExtensions
    {
        public static  async Task ConfigureUserSeedAsync<TApplicationUser, TKey>(this UserManager<TApplicationUser> userManager, Assembly assembly)
            where TApplicationUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var seeds = DbSeed.FromAssembly(assembly);

            foreach (var seed in seeds)
            {
                await userManager.ConfigureSeedAsync(seed);
            }
        }

        public static async Task ConfigureSeedAsync<TApplicationUser, TKey>(this UserManager<TApplicationUser> userManager, IdentitySeed<TApplicationUser, TKey> seed)
            where TApplicationUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            var identitySeeds = (IEnumerable<IdentitySeedModel<TApplicationUser, TKey>>)seed.AllObjects();
            foreach (var identitySeed in identitySeeds)
            {
                // Check if user exists
                var userIdentifier = identitySeed.ApplicationUser.Email;
                var existingUser = await userManager.FindByEmailAsync(userIdentifier);
                if (existingUser != null)
                {
                    // Update user with new details
                    var updateResult = await userManager.UpdateAsync(identitySeed.ApplicationUser);
                    if (!updateResult.Succeeded)
                    {
                        throw new IdentityException(updateResult.Errors, $"Unable to update password on existing user. {userIdentifier}");
                    }

                    // Reset password on seeded user
                    var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(existingUser);
                    var resetResult = await userManager.ResetPasswordAsync(existingUser, passwordResetToken, identitySeed.Password);
                    if (!resetResult.Succeeded)
                    {
                        throw new IdentityException(resetResult.Errors, $"Unable to reset password on existing user. {userIdentifier}");
                    }
                }
                else
                {
                    // Create a new user with password
                    var identityResult = await userManager.CreateAsync(identitySeed.ApplicationUser, identitySeed.Password);
                    if (!identityResult.Succeeded)
                    {
                        throw new IdentityException(identityResult.Errors, $"Unable to create user. {userIdentifier}");
                    }
                }
            }
        }
    }
}
