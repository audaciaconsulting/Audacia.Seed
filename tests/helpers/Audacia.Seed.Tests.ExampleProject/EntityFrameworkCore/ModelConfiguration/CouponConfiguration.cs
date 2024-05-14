using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
    }
}