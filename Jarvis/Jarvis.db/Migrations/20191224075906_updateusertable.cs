using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class updateusertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "firstname",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lastname",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "firstname",
                table: "User");

            migrationBuilder.DropColumn(
                name: "lastname",
                table: "User");
        }
    }
}
