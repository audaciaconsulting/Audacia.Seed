using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class RoomAssetConfiguration : IEntityTypeConfiguration<RoomAsset>
{
    public void Configure(EntityTypeBuilder<RoomAsset> builder)
    {
        builder.HasOne(ca => ca.Room)
            .WithMany()
            .HasForeignKey(ca => ca.RoomId);
    }
}