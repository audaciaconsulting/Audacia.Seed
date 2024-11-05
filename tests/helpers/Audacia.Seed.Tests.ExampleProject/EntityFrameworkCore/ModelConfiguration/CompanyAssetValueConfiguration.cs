using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class CompanyAssetValueConfiguration : IEntityTypeConfiguration<CompanyAssetValue>
{
    public void Configure(EntityTypeBuilder<CompanyAssetValue> builder)
    {
        builder.HasOne(ca => ca.CompanyAsset)
            .WithMany(a => a.Values)
            .HasForeignKey(ca => ca.CompanyAssetId);
    }
}