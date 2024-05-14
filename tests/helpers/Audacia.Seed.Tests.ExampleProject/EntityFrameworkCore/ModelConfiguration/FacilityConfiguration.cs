using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.HasOne(f => f.Room)
            .WithMany(b => b.Facilities)
            .HasForeignKey(f => f.RoomId);

        builder.HasOne(f => f.Pool)
            .WithMany(b => b.Facilities)
            .HasForeignKey(f => f.PoolId);

        builder.HasOne(f => f.Owner)
            .WithMany(e => e.FacilitiesOwned)
            .HasForeignKey(f => f.OwnerId);

        builder.HasOne(f => f.Manager)
            .WithMany(e => e.FacilitiesManaged)
            .HasForeignKey(f => f.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TypeEntity)
            .WithMany(t => t.Facilities)
            .HasForeignKey(t => t.Type)
            .OnDelete(DeleteBehavior.Restrict);
    }
}