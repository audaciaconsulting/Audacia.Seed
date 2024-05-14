using Audacia.Seed.Tests.ExampleProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Audacia.Seed.Tests.ExampleProject.EntityFrameworkCore.ModelConfiguration;

public class FacilityTypeEntityConfiguration : IEntityTypeConfiguration<FacilityTypeEntity>
{
    public void Configure(EntityTypeBuilder<FacilityTypeEntity> builder)
    {
        builder.HasKey(t => t.Type);
        builder.Property(t => t.Type).ValueGeneratedNever();
    }
}