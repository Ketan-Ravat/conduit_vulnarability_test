using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_replace_meppingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "replaced_asset_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetReplacementMapping",
                columns: table => new
                {
                    asset_replacement_mapping_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    replaced_asset_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetReplacementMapping", x => x.asset_replacement_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetReplacementMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetReplacementMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetReplacementMapping_asset_id",
                table: "AssetReplacementMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetReplacementMapping_site_id",
                table: "AssetReplacementMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetReplacementMapping");

            migrationBuilder.DropColumn(
                name: "replaced_asset_id",
                table: "WOOnboardingAssets");
        }
    }
}
