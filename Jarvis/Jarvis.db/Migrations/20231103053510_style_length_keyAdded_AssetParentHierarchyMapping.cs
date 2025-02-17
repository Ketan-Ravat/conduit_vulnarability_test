using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class style_length_keyAdded_AssetParentHierarchyMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "length",
                table: "AssetParentHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "style",
                table: "AssetParentHierarchyMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "length",
                table: "AssetParentHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "style",
                table: "AssetParentHierarchyMapping");
        }
    }
}
