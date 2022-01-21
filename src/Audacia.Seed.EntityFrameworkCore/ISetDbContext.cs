using Microsoft.EntityFrameworkCore;

namespace Audacia.Seed.EntityFrameworkCore
{
    /// <summary>
    /// An interface that allows for the internal database context on a <see cref="SeedFromDatabase{T}"/> to be set.
    /// </summary>
    public interface ISetDbContext
    {
        /// <summary>
        /// Sets the internal database context.
        /// </summary>
        /// <param name="context">An initialised database context.</param>
        void SetDbContext(DbContext context);
    }
}
