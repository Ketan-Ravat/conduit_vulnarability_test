using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AssetChildrenHierarchyMapping_circuit_col_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "circuit",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "circuit",
                table: "AssetChildrenHierarchyMapping");
        }
    }
}
