using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class project_idaddedcompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "autodesk_app_id",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "project_id",
                table: "Company",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "project_id",
                table: "Company");

            migrationBuilder.AddColumn<string>(
                name: "autodesk_app_id",
                table: "Company",
                type: "text",
                nullable: true);
        }
    }
}
