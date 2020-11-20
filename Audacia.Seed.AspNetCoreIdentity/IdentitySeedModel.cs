using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity
{
    public class IdentitySeedModel<TApplicationUser, TKey>
        where TApplicationUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
        public TApplicationUser ApplicationUser { get; set; }

        public string Password { get; set; }

        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}