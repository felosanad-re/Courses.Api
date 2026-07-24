using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSomePropertiesInInstructorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "Instructors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Instructors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Pending");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "Instructors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Instructors");
        }
    }
}
