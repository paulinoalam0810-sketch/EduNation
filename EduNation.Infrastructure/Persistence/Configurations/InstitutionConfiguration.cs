using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
    {
        public void Configure(EntityTypeBuilder<Institution> builder)
        {
            builder.ToTable("Institutions");
            builder.HasKey(institution => institution.Id);
            builder.Property(institution => institution.Id).ValueGeneratedNever();
            builder.Property(institution => institution.Name).IsRequired().HasMaxLength(150);
            builder.Property(institution => institution.Code).IsRequired().HasMaxLength(32);
            builder.HasIndex(institution => institution.Code).IsUnique();
            builder.Property(institution => institution.CreatedAt).IsRequired();
            builder.Property(institution => institution.IsActive).IsRequired();
        }
    }
}
