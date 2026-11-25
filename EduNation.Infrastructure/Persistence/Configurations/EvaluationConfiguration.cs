using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class EvaluationConfiguration : IEntityTypeConfiguration<Evaluation>
    {
        public void Configure(EntityTypeBuilder<Evaluation> builder)
        {
            builder.ToTable("Evaluations");
            builder.HasKey(evaluation => evaluation.Id);
            builder.Property(evaluation => evaluation.Id).ValueGeneratedNever();
            builder.Property(evaluation => evaluation.CourseSectionId).IsRequired();
            builder.Property(evaluation => evaluation.Name).IsRequired().HasMaxLength(150);
            builder.Property(evaluation => evaluation.Description).HasMaxLength(1000);
            builder.Property(evaluation => evaluation.MaxScore).HasPrecision(18, 2).IsRequired();
            builder.Property(evaluation => evaluation.Weight).HasPrecision(5, 2).IsRequired();
            builder.Property(evaluation => evaluation.EvaluationDate).IsRequired();
            builder.Property(evaluation => evaluation.CreatedAt).IsRequired();
            builder.Property(evaluation => evaluation.IsActive).IsRequired();
            builder.Property(evaluation => evaluation.InstitutionId);

            builder.HasOne<Institution>()
                .WithMany()
                .HasForeignKey(evaluation => evaluation.InstitutionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(evaluation => evaluation.CourseSectionId);

            builder.HasOne<CourseSection>()
                .WithMany()
                .HasForeignKey(evaluation => evaluation.CourseSectionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
