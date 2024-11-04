using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class CompanyAssetAttributeConfiguration : IEntityTypeConfiguration<CompanyAssetAttribute>
{
    public void Configure(EntityTypeBuilder<CompanyAsset> builder)
    {
        builder.HasOne(ca => ca.Asset)
            .WithMany(a => a.CompanyAssets)
            .HasForeignKey(ca => ca.AssetId);

        builder.HasOne(ca => ca.Company)
            .WithMany(a => a.Assets)
            .HasForeignKey(ca => ca.CompanyId);
    }

    public void Configure(EntityTypeBuilder<CompanyAssetAttribute> builder)
    {
        builder.HasOne(ca => ca.CompanyAsset)
            .WithMany(a => a.Attributes)
            .HasForeignKey(ca => ca.CompanyAssetId);
    }
}