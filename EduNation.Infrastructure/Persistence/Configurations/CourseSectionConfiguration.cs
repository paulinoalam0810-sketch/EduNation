using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class CourseSectionConfiguration : IEntityTypeConfiguration<CourseSection>
    {
        public void Configure(EntityTypeBuilder<CourseSection> builder)
        {
            builder.ToTable("CourseSections");
            builder.HasKey(courseSection => courseSection.Id);
            builder.Property(courseSection => courseSection.Id).ValueGeneratedNever();
            builder.Property(courseSection => courseSection.CourseId).IsRequired();
            builder.Property(courseSection => courseSection.AcademicPeriodId).IsRequired();
            builder.Property(courseSection => courseSection.TeacherId).IsRequired();
            builder.Property(courseSection => courseSection.Code).IsRequired().HasMaxLength(32);
            builder.Property(courseSection => courseSection.Name).HasMaxLength(150);
            builder.Property(courseSection => courseSection.Capacity).IsRequired();
            builder.Property(courseSection => courseSection.CreatedAt).IsRequired();
            builder.Property(courseSection => courseSection.IsActive).IsRequired();
            builder.Property(courseSection => courseSection.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(courseSection => courseSection.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(courseSection => new { courseSection.CourseId, courseSection.AcademicPeriodId, courseSection.Code })
                .IsUnique()
                .HasFilter("[IsActive] = 1");

            builder.HasOne<Course>()
                .WithMany()
                .HasForeignKey(courseSection => courseSection.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<AcademicPeriod>()
                .WithMany()
                .HasForeignKey(courseSection => courseSection.AcademicPeriodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Teacher>()
                .WithMany()
                .HasForeignKey(courseSection => courseSection.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
