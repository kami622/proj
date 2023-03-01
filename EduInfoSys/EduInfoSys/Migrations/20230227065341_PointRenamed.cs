using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduInfoSys.Migrations
{
    public partial class PointRenamed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "TaskGrades",
                newName: "Point");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Point",
                table: "TaskGrades",
                newName: "Points");
        }
    }
}
