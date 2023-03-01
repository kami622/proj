using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduInfoSys.Migrations
{
    public partial class LastSignInColumnAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSignIn",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSignIn",
                table: "AspNetUsers");
        }
    }
}
