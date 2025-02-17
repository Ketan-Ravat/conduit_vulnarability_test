using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOOBAssetTempLocationMappingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WOOBAssetTempFormIOBuildingMappings",
                columns: table => new
                {
                    wo_ob_asset_temp_formiobuilding_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    temp_formiobuilding_id = table.Column<Guid>(nullable: true),
                    temp_formiofloor_id = table.Column<Guid>(nullable: true),
                    temp_formioroom_id = table.Column<Guid>(nullable: true),
                    temp_formiosection_id = table.Column<Guid>(nullable: true),
                    woonboardingassets_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOOBAssetTempFormIOBuildingMappings", x => x.wo_ob_asset_temp_formiobuilding_id);
                    table.ForeignKey(
                        name: "FK_WOOBAssetTempFormIOBuildingMappings_TempFormIOBuildings_tem~",
                        column: x => x.temp_formiobuilding_id,
                        principalTable: "TempFormIOBuildings",
                        principalColumn: "temp_formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOOBAssetTempFormIOBuildingMappings_TempFormIOFloors_temp_f~",
                        column: x => x.temp_formiofloor_id,
                        principalTable: "TempFormIOFloors",
                        principalColumn: "temp_formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOOBAssetTempFormIOBuildingMappings_TempFormIORooms_temp_fo~",
                        column: x => x.temp_formioroom_id,
                        principalTable: "TempFormIORooms",
                        principalColumn: "temp_formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOOBAssetTempFormIOBuildingMappings_TempFormIOSections_temp~",
                        column: x => x.temp_formiosection_id,
                        principalTable: "TempFormIOSections",
                        principalColumn: "temp_formiosection_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOOBAssetTempFormIOBuildingMappings_WOOnboardingAssets_woon~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetTempFormIOBuildingMappings_temp_formiobuilding_id",
                table: "WOOBAssetTempFormIOBuildingMappings",
                column: "temp_formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetTempFormIOBuildingMappings_temp_formiofloor_id",
                table: "WOOBAssetTempFormIOBuildingMappings",
                column: "temp_formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetTempFormIOBuildingMappings_temp_formioroom_id",
                table: "WOOBAssetTempFormIOBuildingMappings",
                column: "temp_formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetTempFormIOBuildingMappings_temp_formiosection_id",
                table: "WOOBAssetTempFormIOBuildingMappings",
                column: "temp_formiosection_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetTempFormIOBuildingMappings_woonboardingassets_id",
                table: "WOOBAssetTempFormIOBuildingMappings",
                column: "woonboardingassets_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOOBAssetTempFormIOBuildingMappings");
        }
    }
}
