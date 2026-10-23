using System.Diagnostics.CodeAnalysis;

namespace Audacia.Seed.Tests.ExampleProject.Entities;

[method: SetsRequiredMembers]
public class Coupon(string name)
{
    public int Id { get; set; }

    public required string Name { get; set; } = name;

    public decimal Discount { get; set; }

    public Booking? Booking { get; set; }
}