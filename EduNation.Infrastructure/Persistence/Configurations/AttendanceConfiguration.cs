using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");
            builder.HasKey(attendance => attendance.Id);
            builder.Property(attendance => attendance.Id).ValueGeneratedNever();
            builder.Property(attendance => attendance.EnrollmentId).IsRequired();
            builder.Property(attendance => attendance.Date).IsRequired();
            builder.Property(attendance => attendance.Status).IsRequired();
            builder.Property(attendance => attendance.CreatedAt).IsRequired();
            builder.Property(attendance => attendance.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(attendance => attendance.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(attendance => new { attendance.EnrollmentId, attendance.Date })
                .IsUnique();

            builder.HasOne<Enrollment>()
                .WithMany()
                .HasForeignKey(attendance => attendance.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
