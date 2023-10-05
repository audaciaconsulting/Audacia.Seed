using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity
{
    /// <summary>
    /// Model for seeding application users.
    /// </summary>
    /// <typeparam name="TApplicationUser">The application user type for the database.</typeparam>
    /// <typeparam name="TKey">The type of the identifier for the user e.g. int.</typeparam>
    public class IdentitySeedModel<TApplicationUser, TKey>
        where TApplicationUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the user for the seed.
        /// </summary>
        public TApplicationUser ApplicationUser { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password for the seed.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets the roles to be assigned to the identity seed.
        /// </summary>
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}