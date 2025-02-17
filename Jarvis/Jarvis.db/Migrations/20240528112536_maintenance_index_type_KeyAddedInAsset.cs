using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class maintenance_index_type_KeyAddedInAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "maintenance_index_type",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "maintenance_index_type",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "maintenance_index_type",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "maintenance_index_type",
                table: "Assets");
        }
    }
}
