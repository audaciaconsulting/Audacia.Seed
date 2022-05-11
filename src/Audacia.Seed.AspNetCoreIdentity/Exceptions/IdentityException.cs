using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audacia.Seed.AspNetCoreIdentity.Exceptions
{
    /// <summary>
    /// Exception type for errors when configuring the database with seeds related to user identitifcation.
    /// </summary>
    public sealed class IdentityException : Exception
    {
        /// <summary>
        /// Constructor for <see cref="IdentityException"/> providing a collection of <see cref="IdentityError"/>s.
        /// </summary>
        /// <param name="errors">A collection of <see cref="IdentityError"/>s.</param>
        /// <param name="message">An error message.</param>
        public IdentityException(IEnumerable<IdentityError> errors, string message)
            : base(message)
        {
            if (errors == null)
            {
                throw new ArgumentNullException(nameof(errors));
            }

            foreach (var identityError in errors)
            {
                Data.Add(identityError.Code, identityError.Description);
            }
        }

        /// <summary>
        /// Parameterless constructor for <see cref="IdentityException"/>.
        /// </summary>
        public IdentityException()
        {
        }

        /// <summary>
        /// Constructor for <see cref="IdentityException"/> providing an error message.
        /// </summary>
        /// <param name="message">An error message.</param>
        public IdentityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor for <see cref="IdentityException"/> providing an error message and an inner exception.
        /// </summary>
        /// <param name="message">An error message.</param>
        /// <param name="innerException">An inner exception for the <see cref="IdentityException"/>.</param>
        public IdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}