using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduInfoSys.Migrations
{
    public partial class TeacherColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "SubjectSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubjectSources_TeacherId",
                table: "SubjectSources",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectSources_SubjectTeachers_TeacherId",
                table: "SubjectSources",
                column: "TeacherId",
                principalTable: "SubjectTeachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectSources_SubjectTeachers_TeacherId",
                table: "SubjectSources");

            migrationBuilder.DropIndex(
                name: "IX_SubjectSources_TeacherId",
                table: "SubjectSources");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "SubjectSources");
        }
    }
}
