using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");

            builder.HasKey(teacher => teacher.Id);

            builder.Property(teacher => teacher.Id)
                .ValueGeneratedNever();

            builder.Property(teacher => teacher.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(teacher => teacher.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(teacher => teacher.Email)
                .IsRequired()
                .HasMaxLength(254);

            builder.HasIndex(teacher => teacher.Email)
                .IsUnique();

            builder.Property(teacher => teacher.PhoneNumber)
                .HasMaxLength(32);

            builder.Property(teacher => teacher.CreatedAt)
                .IsRequired();

            builder.Property(teacher => teacher.IsActive)
                .IsRequired();

            builder.Property(teacher => teacher.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(teacher => teacher.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
