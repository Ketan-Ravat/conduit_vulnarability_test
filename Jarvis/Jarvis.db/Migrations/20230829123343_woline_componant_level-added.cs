using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class woline_componant_leveladded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "componant_level_type_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fed_by_usage_type_id",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_via_subcomponant_asset_from_ob_wo",
                table: "WOOBAssetFedByMapping",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "via_subcomponant_asset_id",
                table: "WOOBAssetFedByMapping",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WOlineSubLevelcomponantMapping",
                columns: table => new
                {
                    woline_sublevelcomponant_mapping_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    sublevelcomponant_asset_id = table.Column<Guid>(nullable: false),
                    is_sublevelcomponant_from_ob_wo = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineSubLevelcomponantMapping", x => x.woline_sublevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponantMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WOlineTopLevelcomponantMapping",
                columns: table => new
                {
                    woline_toplevelcomponant_mapping_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    toplevelcomponant_asset_id = table.Column<Guid>(nullable: false),
                    is_toplevelcomponant_from_ob_wo = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineTopLevelcomponantMapping", x => x.woline_toplevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponantMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponantMapping_site_id",
                table: "WOlineSubLevelcomponantMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponantMapping_woonboardingassets_id",
                table: "WOlineSubLevelcomponantMapping",
                column: "woonboardingassets_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponantMapping_site_id",
                table: "WOlineTopLevelcomponantMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponantMapping_woonboardingassets_id",
                table: "WOlineTopLevelcomponantMapping",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOlineSubLevelcomponantMapping");

            migrationBuilder.DropTable(
                name: "WOlineTopLevelcomponantMapping");

            migrationBuilder.DropColumn(
                name: "componant_level_type_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "fed_by_usage_type_id",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "is_via_subcomponant_asset_from_ob_wo",
                table: "WOOBAssetFedByMapping");

            migrationBuilder.DropColumn(
                name: "via_subcomponant_asset_id",
                table: "WOOBAssetFedByMapping");
        }
    }
}
