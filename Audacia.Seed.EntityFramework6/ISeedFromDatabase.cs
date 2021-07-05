using System.Data.Entity;

namespace Audacia.Seed.EntityFramework6
{
    /// <summary>
    /// Interface that seed classes can inherit from (in addition to inheriting from DbSeed). These seed classes will be able to access the database via their DbContext property.
    /// </summary>
    public interface ISeedFromDatabase
    {
        /// <summary>
        /// Gets or sets the DbContext for the seed class.
        /// </summary>
        DbContext DbContext { get; set; }
    }
}
