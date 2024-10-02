using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class CouponIssuerConfiguration : IEntityTypeConfiguration<CouponIssuer>
{
    public void Configure(EntityTypeBuilder<CouponIssuer> builder)
    {
        builder.HasKey(ci => new { ci.CouponId, ci.IssuerId });
    }
}