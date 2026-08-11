using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterInCoursesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "RecorderCourse");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "RecorderCourse",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
