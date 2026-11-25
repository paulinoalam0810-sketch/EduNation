using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grades");
            builder.HasKey(grade => grade.Id);
            builder.Property(grade => grade.Id).ValueGeneratedNever();
            builder.Property(grade => grade.EvaluationId).IsRequired();
            builder.Property(grade => grade.EnrollmentId).IsRequired();
            builder.Property(grade => grade.Score).HasPrecision(18, 2).IsRequired();
            builder.Property(grade => grade.Comments).HasMaxLength(1000);
            builder.Property(grade => grade.CreatedAt).IsRequired();
            builder.Property(grade => grade.UpdatedAt).IsRequired();
            builder.Property(grade => grade.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(grade => grade.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(grade => new { grade.EvaluationId, grade.EnrollmentId })
                .IsUnique();

            builder.HasOne<Evaluation>()
                .WithMany()
                .HasForeignKey(grade => grade.EvaluationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Enrollment>()
                .WithMany()
                .HasForeignKey(grade => grade.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
