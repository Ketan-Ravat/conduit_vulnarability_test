using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class job_title_signUrl_AddKeysIn_UserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "job_title",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "signature_url",
                table: "User",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "job_title",
                table: "User");

            migrationBuilder.DropColumn(
                name: "signature_url",
                table: "User");
        }
    }
}
