using System.Data.Entity;

namespace Audacia.Seed.EntityFramework6
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public interface ISeedFromDatabase
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        DbContext DbContext { get; set; }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
