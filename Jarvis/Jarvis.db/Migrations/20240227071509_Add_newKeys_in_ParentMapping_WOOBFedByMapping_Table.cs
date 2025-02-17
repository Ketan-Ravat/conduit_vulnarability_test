using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Add_newKeys_in_ParentMapping_WOOBFedByMapping_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "conductor_type_id",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_conductor",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "raceway_type_id",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "conductor_type_id",
                table: "AssetParentHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "number_of_conductor",
                table: "AssetParentHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "raceway_type_id",
                table: "AssetParentHierarchyMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conductor_type_id",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "number_of_conductor",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "raceway_type_id",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "conductor_type_id",
                table: "AssetParentHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "number_of_conductor",
                table: "AssetParentHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "raceway_type_id",
                table: "AssetParentHierarchyMapping");
        }
    }
}
