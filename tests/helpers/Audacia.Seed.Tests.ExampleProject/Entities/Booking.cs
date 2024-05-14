namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Booking
{
    public int Id { get; set; }

    public DateTimeOffset Start { get; set; }

    public DateTimeOffset Finish { get; set; }

    public string? Notes { get; set; }

    public int FacilityId { get; set; }

    public Facility Facility { get; set; } = null!;

    public int MemberId { get; set; }

    public Member Member { get; set; } = null!;

    public int? CouponId { get; set; }

    public Coupon? Coupon { get; set; }
}