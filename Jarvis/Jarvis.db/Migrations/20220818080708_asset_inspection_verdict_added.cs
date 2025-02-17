using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_inspection_verdict_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "inspection_verdict",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "inspection_verdict",
                table: "AssetFormIO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inspection_verdict",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "inspection_verdict",
                table: "AssetFormIO");
        }
    }
}
