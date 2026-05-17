using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableStudentLectureProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StudentLectureProgresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StudentLectureProgresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StudentLectureProgresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessedAt",
                table: "StudentLectureProgresses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "LastWatchedSeconds",
                table: "StudentLectureProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "VideoDuration",
                table: "StudentLectureProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StudentLectureProgresses");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StudentLectureProgresses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StudentLectureProgresses");

            migrationBuilder.DropColumn(
                name: "LastAccessedAt",
                table: "StudentLectureProgresses");

            migrationBuilder.DropColumn(
                name: "LastWatchedSeconds",
                table: "StudentLectureProgresses");

            migrationBuilder.DropColumn(
                name: "VideoDuration",
                table: "StudentLectureProgresses");
        }
    }
}
