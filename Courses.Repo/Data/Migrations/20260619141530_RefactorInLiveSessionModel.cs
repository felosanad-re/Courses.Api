using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefactorInLiveSessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveSession_Courses_CourseId",
                table: "LiveSession");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "LiveSession",
                newName: "SectionId");

            migrationBuilder.RenameIndex(
                name: "IX_LiveSession_CourseId",
                table: "LiveSession",
                newName: "IX_LiveSession_SectionId");

            migrationBuilder.AlterColumn<string>(
                name: "RecordingUrl",
                table: "LiveSession",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Topic",
                table: "LiveSession",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_LiveSession_Sections_SectionId",
                table: "LiveSession",
                column: "SectionId",
                principalTable: "Sections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiveSession_Sections_SectionId",
                table: "LiveSession");

            migrationBuilder.DropColumn(
                name: "Topic",
                table: "LiveSession");

            migrationBuilder.RenameColumn(
                name: "SectionId",
                table: "LiveSession",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_LiveSession_SectionId",
                table: "LiveSession",
                newName: "IX_LiveSession_CourseId");

            migrationBuilder.AlterColumn<string>(
                name: "RecordingUrl",
                table: "LiveSession",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LiveSession_Courses_CourseId",
                table: "LiveSession",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
