using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasOne(b => b.Member)
            .WithMany(m => m.Bookings)
            .HasForeignKey(b => b.MemberId);

        builder.HasOne(b => b.Facility)
            .WithMany(f => f.Bookings)
            .HasForeignKey(b => b.FacilityId);

        builder.HasOne(b => b.Coupon)
            .WithOne(c => c.Booking)
            .HasForeignKey<Booking>(b => b.CouponId);
    }
}