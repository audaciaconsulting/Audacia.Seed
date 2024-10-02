using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class MembershipGroupConfiguration : IEntityTypeConfiguration<MembershipGroup>
{
    public void Configure(EntityTypeBuilder<MembershipGroup> builder)
    {
        builder.HasOne(m => m.Region)
            .WithMany(r => r.MembershipGroups)
            .HasForeignKey(m => m.RegionId);

        builder.HasOne(m => m.Parent)
            .WithMany(mg => mg.ChildGroups)
            .HasForeignKey(m => m.ParentId);
    }
}