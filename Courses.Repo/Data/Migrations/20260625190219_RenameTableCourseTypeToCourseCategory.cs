using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableCourseTypeToCourseCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseTypes_CourseTypeId",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "CourseTypes",
                newName: "CourseCategories");

            migrationBuilder.RenameColumn(
                name: "CourseTypeId",
                table: "Courses",
                newName: "CourseCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_CourseTypeId",
                table: "Courses",
                newName: "IX_Courses_CourseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseCategories_CourseCategoryId",
                table: "Courses",
                column: "CourseCategoryId",
                principalTable: "CourseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseCategories_CourseCategoryId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "CourseCategoryId",
                table: "Courses",
                newName: "CourseTypeId");

            migrationBuilder.RenameTable(
                name: "CourseCategories",
                newName: "CourseTypes");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_CourseCategoryId",
                table: "Courses",
                newName: "IX_Courses_CourseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseTypes_CourseTypeId",
                table: "Courses",
                column: "CourseTypeId",
                principalTable: "CourseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
