using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasOne(m => m.MembershipGroup)
            .WithMany(mg => mg.Members)
            .HasForeignKey(m => m.MembershipGroupId);
    }
}