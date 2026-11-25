using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnrollmentsForCourseSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AcademicPeriods_AcademicPeriodId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_AcademicPeriodId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseId_AcademicPeriodId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicPeriodId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CourseSectionId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseSectionId",
                table: "Enrollments",
                column: "CourseSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseSectionId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseSectionId" },
                unique: true,
                filter: "[CourseSectionId] IS NOT NULL AND [Status] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_CourseSections_CourseSectionId",
                table: "Enrollments",
                column: "CourseSectionId",
                principalTable: "CourseSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_CourseSections_CourseSectionId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseSectionId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_StudentId_CourseSectionId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CourseSectionId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<Guid>(
                name: "CourseId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicPeriodId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_AcademicPeriodId",
                table: "Enrollments",
                column: "AcademicPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseId",
                table: "Enrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_StudentId_CourseId_AcademicPeriodId",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId", "AcademicPeriodId" },
                unique: true,
                filter: "[Status] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AcademicPeriods_AcademicPeriodId",
                table: "Enrollments",
                column: "AcademicPeriodId",
                principalTable: "AcademicPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CourseId",
                table: "Enrollments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
