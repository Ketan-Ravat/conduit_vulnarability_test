using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset__status_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status_name",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "asset_operating_condition_state",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "asset_placement",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mwo_inspection_type_status",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_operating_condition_state",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "asset_placement",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "mwo_inspection_type_status",
                table: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "status_name",
                table: "Assets",
                type: "text",
                nullable: true);
        }
    }
}
