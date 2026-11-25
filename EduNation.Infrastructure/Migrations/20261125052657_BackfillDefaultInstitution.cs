using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BackfillDefaultInstitution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM Institutions WHERE Id = '00000000-0000-0000-0000-000000000001')
BEGIN
    INSERT INTO Institutions (Id, Name, Code, CreatedAt, IsActive)
    VALUES ('00000000-0000-0000-0000-000000000001', 'Default Institution', 'DEFAULT', SYSDATETIMEOFFSET(), 1)
END
");

            migrationBuilder.Sql("UPDATE Students SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Teachers SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Courses SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE AcademicPeriods SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE CourseSections SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Enrollments SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Attendances SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Evaluations SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
            migrationBuilder.Sql("UPDATE Grades SET InstitutionId = '00000000-0000-0000-0000-000000000001' WHERE InstitutionId IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
