using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class CompanyAssetConfiguration : IEntityTypeConfiguration<CompanyAsset>
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
}