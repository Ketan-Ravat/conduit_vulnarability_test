using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_parent_childrenadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "fed_by_usage_type_id",
                table: "AssetParentHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "via_subcomponent_asset_id",
                table: "AssetParentHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "via_subcomponent_asset_id",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fed_by_usage_type_id",
                table: "AssetParentHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "via_subcomponent_asset_id",
                table: "AssetParentHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "via_subcomponent_asset_id",
                table: "AssetChildrenHierarchyMapping");
        }
    }
}
