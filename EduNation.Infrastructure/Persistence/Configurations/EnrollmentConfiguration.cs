using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");
            builder.HasKey(enrollment => enrollment.Id);
            builder.Property(enrollment => enrollment.Id).ValueGeneratedNever();
            builder.Property(enrollment => enrollment.StudentId).IsRequired();
            builder.Property(enrollment => enrollment.CourseSectionId);
            builder.Property(enrollment => enrollment.CourseId);
            builder.Property(enrollment => enrollment.AcademicPeriodId);
            builder.Property(enrollment => enrollment.EnrolledAt).IsRequired();
            builder.Property(enrollment => enrollment.Status).IsRequired();
            builder.Property(enrollment => enrollment.CreatedAt).IsRequired();
            builder.Property(enrollment => enrollment.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(enrollment => enrollment.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(enrollment => new { enrollment.StudentId, enrollment.CourseSectionId })
                .IsUnique()
                .HasFilter("[CourseSectionId] IS NOT NULL AND [Status] = 0");

            builder.HasOne<CourseSection>()
                .WithMany()
                .HasForeignKey(enrollment => enrollment.CourseSectionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Student>()
                .WithMany()
                .HasForeignKey(enrollment => enrollment.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
