using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduNation.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionIdToAcademicEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Grades",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Evaluations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Enrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "CourseSections",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "Attendances",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstitutionId",
                table: "AcademicPeriods",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_InstitutionId",
                table: "Teachers",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_InstitutionId",
                table: "Students",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_InstitutionId",
                table: "Grades",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_InstitutionId",
                table: "Evaluations",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_InstitutionId",
                table: "Enrollments",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSections_InstitutionId",
                table: "CourseSections",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_InstitutionId",
                table: "Attendances",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicPeriods_InstitutionId",
                table: "AcademicPeriods",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicPeriods_Institutions_InstitutionId",
                table: "AcademicPeriods",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Institutions_InstitutionId",
                table: "Attendances",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Institutions_InstitutionId",
                table: "Courses",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseSections_Institutions_InstitutionId",
                table: "CourseSections",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Institutions_InstitutionId",
                table: "Enrollments",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Institutions_InstitutionId",
                table: "Evaluations",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Institutions_InstitutionId",
                table: "Grades",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Institutions_InstitutionId",
                table: "Students",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Institutions_InstitutionId",
                table: "Teachers",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicPeriods_Institutions_InstitutionId",
                table: "AcademicPeriods");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Institutions_InstitutionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Institutions_InstitutionId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseSections_Institutions_InstitutionId",
                table: "CourseSections");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Institutions_InstitutionId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Institutions_InstitutionId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Institutions_InstitutionId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Institutions_InstitutionId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Institutions_InstitutionId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_InstitutionId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_InstitutionId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Grades_InstitutionId",
                table: "Grades");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_InstitutionId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_InstitutionId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_CourseSections_InstitutionId",
                table: "CourseSections");

            migrationBuilder.DropIndex(
                name: "IX_Courses_InstitutionId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_InstitutionId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_AcademicPeriods_InstitutionId",
                table: "AcademicPeriods");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "AcademicPeriods");
        }
    }
}
