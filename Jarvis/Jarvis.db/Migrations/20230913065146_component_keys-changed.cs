using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class component_keyschanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetTopLevelcomponentMapping",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetSubLevelcomponentMapping",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "asset_toplevelcomponant_mapping_id",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "toplevelcomponant_asset_id",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "asset_sublevelcomponant_mapping_id",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "sublevelcomponant_asset_id",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_toplevelcomponent_mapping_id",
                table: "AssetTopLevelcomponentMapping",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "toplevelcomponent_asset_id",
                table: "AssetTopLevelcomponentMapping",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "asset_sublevelcomponent_mapping_id",
                table: "AssetSubLevelcomponentMapping",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sublevelcomponent_asset_id",
                table: "AssetSubLevelcomponentMapping",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetTopLevelcomponentMapping",
                table: "AssetTopLevelcomponentMapping",
                column: "asset_toplevelcomponent_mapping_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetSubLevelcomponentMapping",
                table: "AssetSubLevelcomponentMapping",
                column: "asset_sublevelcomponent_mapping_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetTopLevelcomponentMapping",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetSubLevelcomponentMapping",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "asset_toplevelcomponent_mapping_id",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "toplevelcomponent_asset_id",
                table: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "asset_sublevelcomponent_mapping_id",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "sublevelcomponent_asset_id",
                table: "AssetSubLevelcomponentMapping");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_toplevelcomponant_mapping_id",
                table: "AssetTopLevelcomponentMapping",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "toplevelcomponant_asset_id",
                table: "AssetTopLevelcomponentMapping",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "asset_sublevelcomponant_mapping_id",
                table: "AssetSubLevelcomponentMapping",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "sublevelcomponant_asset_id",
                table: "AssetSubLevelcomponentMapping",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetTopLevelcomponentMapping",
                table: "AssetTopLevelcomponentMapping",
                column: "asset_toplevelcomponant_mapping_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetSubLevelcomponentMapping",
                table: "AssetSubLevelcomponentMapping",
                column: "asset_sublevelcomponant_mapping_id");
        }
    }
}
