using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class AssetTypeConfiguration : IEntityTypeConfiguration<AssetType>
{
    public void Configure(EntityTypeBuilder<AssetType> builder)
    {
        builder.HasMany(at => at.Assets)
            .WithOne(a => a.AssetType)
            .HasForeignKey(a => a.AssetTypeId);
    }
}