using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class fedby_via_subcomponentid_keyWOOBFedByMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "fed_by_via_subcomponant_asset_id",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_fed_by_via_subcomponant_asset_from_ob_wo",
                table: "WOOBAssetFedByMapping",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "fed_by_via_subcomponant_asset_id",
                table: "AssetParentHierarchyMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fed_by_via_subcomponant_asset_id",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "is_fed_by_via_subcomponant_asset_from_ob_wo",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "fed_by_via_subcomponant_asset_id",
                table: "AssetParentHierarchyMapping");
        }
    }
}
