using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");
            builder.HasKey(course => course.Id);
            builder.Property(course => course.Id).ValueGeneratedNever();
            builder.Property(course => course.Code).IsRequired().HasMaxLength(32);
            builder.HasIndex(course => course.Code).IsUnique();
            builder.Property(course => course.Name).IsRequired().HasMaxLength(150);
            builder.Property(course => course.Description).HasMaxLength(1000);
            builder.Property(course => course.Credits).IsRequired();
            builder.Property(course => course.CreatedAt).IsRequired();
            builder.Property(course => course.IsActive).IsRequired();
            builder.Property(course => course.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(course => course.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
