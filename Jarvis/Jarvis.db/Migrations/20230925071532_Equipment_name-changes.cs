using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Equipment_namechanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "equipment_name",
                table: "Equipment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "equipment_name",
                table: "Equipment");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Equipment",
                type: "text",
                nullable: true);
        }
    }
}
