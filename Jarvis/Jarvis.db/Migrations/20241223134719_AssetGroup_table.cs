using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AssetGroup_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_group_id",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "asset_group_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetGroup",
                columns: table => new
                {
                    asset_group_id = table.Column<Guid>(nullable: false),
                    asset_group_name = table.Column<string>(nullable: true),
                    asset_group_description = table.Column<string>(nullable: true),
                    criticality_index_type = table.Column<int>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetGroup", x => x.asset_group_id);
                    table.ForeignKey(
                        name: "FK_AssetGroup_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_asset_group_id",
                table: "TempAsset",
                column: "asset_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_asset_group_id",
                table: "Assets",
                column: "asset_group_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetGroup_site_id",
                table: "AssetGroup",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_AssetGroup_asset_group_id",
                table: "Assets",
                column: "asset_group_id",
                principalTable: "AssetGroup",
                principalColumn: "asset_group_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TempAsset_AssetGroup_asset_group_id",
                table: "TempAsset",
                column: "asset_group_id",
                principalTable: "AssetGroup",
                principalColumn: "asset_group_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_AssetGroup_asset_group_id",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_TempAsset_AssetGroup_asset_group_id",
                table: "TempAsset");

            migrationBuilder.DropTable(
                name: "AssetGroup");

            migrationBuilder.DropIndex(
                name: "IX_TempAsset_asset_group_id",
                table: "TempAsset");

            migrationBuilder.DropIndex(
                name: "IX_Assets_asset_group_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "asset_group_id",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "asset_group_id",
                table: "Assets");
        }
    }
}
