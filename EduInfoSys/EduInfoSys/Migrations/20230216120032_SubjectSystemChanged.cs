using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduInfoSys.Migrations
{
    public partial class SubjectSystemChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectTeachers_GroupMembers_GroupMemberId",
                table: "SubjectTeachers");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropIndex(
                name: "IX_SubjectTeachers_GroupMemberId",
                table: "SubjectTeachers");

            migrationBuilder.DropColumn(
                name: "GroupMemberId",
                table: "SubjectTeachers");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SubjectTeachers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupStudents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupStudents_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTeachers_UserId",
                table: "SubjectTeachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_GroupId",
                table: "GroupStudents",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_UserId",
                table: "GroupStudents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectTeachers_AspNetUsers_UserId",
                table: "SubjectTeachers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectTeachers_AspNetUsers_UserId",
                table: "SubjectTeachers");

            migrationBuilder.DropTable(
                name: "GroupStudents");

            migrationBuilder.DropIndex(
                name: "IX_SubjectTeachers_UserId",
                table: "SubjectTeachers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SubjectTeachers");

            migrationBuilder.AddColumn<int>(
                name: "GroupMemberId",
                table: "SubjectTeachers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTeacher = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTeachers_GroupMemberId",
                table: "SubjectTeachers",
                column: "GroupMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId",
                table: "GroupMembers",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_UserId",
                table: "GroupMembers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectTeachers_GroupMembers_GroupMemberId",
                table: "SubjectTeachers",
                column: "GroupMemberId",
                principalTable: "GroupMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
