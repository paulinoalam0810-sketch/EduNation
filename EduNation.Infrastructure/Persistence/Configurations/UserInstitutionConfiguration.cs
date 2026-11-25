using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence.Configurations
{
    public class UserInstitutionConfiguration : IEntityTypeConfiguration<UserInstitution>
    {
        public void Configure(EntityTypeBuilder<UserInstitution> builder)
        {
            builder.ToTable("UserInstitutions");
            builder.HasKey(userInstitution => new { userInstitution.UserId, userInstitution.InstitutionId });
            builder.Property(userInstitution => userInstitution.UserId).IsRequired().HasMaxLength(450);
            builder.Property(userInstitution => userInstitution.InstitutionId).IsRequired();
            builder.Property(userInstitution => userInstitution.Role).IsRequired().HasMaxLength(50);
            builder.Property(userInstitution => userInstitution.CreatedAt).IsRequired();
        }
    }
}
