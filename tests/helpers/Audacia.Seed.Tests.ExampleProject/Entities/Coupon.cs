using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

public class Coupon
{
    [SetsRequiredMembers]
    public Coupon(string name)
    {
        Name = name;
    }

    public int Id { get; set; }

    public required string Name { get; set; }

    public decimal Discount { get; set; }

    public Booking? Booking { get; set; }
}