using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Repo.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterInTableInstructorRequestRemoveNFPFOrInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstructorRequests_Instructors_InstructorId",
                table: "InstructorRequests");

            migrationBuilder.DropIndex(
                name: "IX_InstructorRequests_InstructorId",
                table: "InstructorRequests");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "InstructorRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "InstructorRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InstructorRequests_InstructorId",
                table: "InstructorRequests",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_InstructorRequests_Instructors_InstructorId",
                table: "InstructorRequests",
                column: "InstructorId",
                principalTable: "Instructors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
