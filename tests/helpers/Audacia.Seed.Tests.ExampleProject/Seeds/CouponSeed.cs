using Audacia.Seed.Tests.ExampleProject.Entities;

namespace Audacia.Seed.Tests.ExampleProject.Seeds;

public class CouponSeed : EntitySeed<Coupon>
{
    protected override Coupon GetDefault(int index, Coupon? previous)
    {
        return new Coupon(Guid.NewGuid().ToString())
        {
            Discount = 0.1m
        };
    }
}