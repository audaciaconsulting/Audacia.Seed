using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class PoolAssetConfiguration : IEntityTypeConfiguration<PoolAsset>
{
    public void Configure(EntityTypeBuilder<PoolAsset> builder)
    {
        builder.HasOne(ca => ca.Pool)
            .WithMany()
            .HasForeignKey(ca => ca.PoolId);
    }
}