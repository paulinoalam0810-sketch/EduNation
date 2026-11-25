using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduNation.Domain.Entities;

namespace EduNation.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<AcademicPeriod> AcademicPeriods => Set<AcademicPeriod>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<CourseSection> CourseSections => Set<CourseSection>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Evaluation> Evaluations => Set<Evaluation>();
        public DbSet<Grade> Grades => Set<Grade>();
        public DbSet<Institution> Institutions => Set<Institution>();
        public DbSet<UserInstitution> UserInstitutions => Set<UserInstitution>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
