namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class CouponIssuer
{
    public int CouponId { get; set; }

    public Coupon Coupon { get; set; } = null!;

    public int IssuerId { get; set; }

    public Employee Issuer { get; set; } = null!;

    public DateTime IssuedOn { get; set; }
}