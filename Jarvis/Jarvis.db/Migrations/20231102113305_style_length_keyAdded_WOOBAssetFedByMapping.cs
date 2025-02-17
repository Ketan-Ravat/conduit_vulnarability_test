using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class style_length_keyAdded_WOOBAssetFedByMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "length",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "style",
                table: "WOOBAssetFedByMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "length",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "style",
                table: "WOOBAssetFedByMapping");
        }
    }
}
