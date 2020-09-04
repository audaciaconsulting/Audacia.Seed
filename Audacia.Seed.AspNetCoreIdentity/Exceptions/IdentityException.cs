using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity.Exceptions
{
    public sealed class IdentityException : Exception
    {
        public IdentityException(IEnumerable<IdentityError> errors, string message)
            : base(message)
        {
            foreach (var identityError in errors)
            {
                Data.Add(identityError.Code, identityError.Description);
            }
        }
    }
}