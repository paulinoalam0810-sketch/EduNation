using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class AcademicPeriodConfiguration : IEntityTypeConfiguration<AcademicPeriod>
    {
        public void Configure(EntityTypeBuilder<AcademicPeriod> builder)
        {
            builder.ToTable("AcademicPeriods");
            builder.HasKey(academicPeriod => academicPeriod.Id);
            builder.Property(academicPeriod => academicPeriod.Id).ValueGeneratedNever();
            builder.Property(academicPeriod => academicPeriod.Name).IsRequired().HasMaxLength(150);
            builder.Property(academicPeriod => academicPeriod.Code).IsRequired().HasMaxLength(32);
            builder.HasIndex(academicPeriod => academicPeriod.Code).IsUnique();
            builder.Property(academicPeriod => academicPeriod.StartDate).IsRequired();
            builder.Property(academicPeriod => academicPeriod.EndDate).IsRequired();
            builder.Property(academicPeriod => academicPeriod.CreatedAt).IsRequired();
            builder.Property(academicPeriod => academicPeriod.IsActive).IsRequired();
            builder.Property(academicPeriod => academicPeriod.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(academicPeriod => academicPeriod.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
