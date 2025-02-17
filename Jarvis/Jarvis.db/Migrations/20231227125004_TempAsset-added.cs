using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TempAssetadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "tempasset_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TempAsset",
                columns: table => new
                {
                    tempasset_id = table.Column<Guid>(nullable: false),
                    asset_name = table.Column<string>(nullable: true),
                    QR_code = table.Column<string>(nullable: true),
                    condition_index_type = table.Column<int>(nullable: true),
                    criticality_index_type = table.Column<int>(nullable: true),
                    commisiion_date = table.Column<DateTime>(nullable: true),
                    form_nameplate_info = table.Column<string>(nullable: true),
                    component_level_type_id = table.Column<int>(nullable: true),
                    asset_operating_condition_state = table.Column<int>(nullable: true),
                    code_compliance = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true),
                    inspectiontemplate_asset_class_id = table.Column<Guid>(nullable: true),
                    temp_formiobuilding_id = table.Column<Guid>(nullable: true),
                    temp_formiofloor_id = table.Column<Guid>(nullable: true),
                    temp_formioroom_id = table.Column<Guid>(nullable: true),
                    temp_formiosection_id = table.Column<Guid>(nullable: true),
                    wo_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempAsset", x => x.tempasset_id);
                    table.ForeignKey(
                        name: "FK_TempAsset_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_InspectionTemplateAssetClass_inspectiontemplate_a~",
                        column: x => x.inspectiontemplate_asset_class_id,
                        principalTable: "InspectionTemplateAssetClass",
                        principalColumn: "inspectiontemplate_asset_class_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempAsset_TempFormIOBuildings_temp_formiobuilding_id",
                        column: x => x.temp_formiobuilding_id,
                        principalTable: "TempFormIOBuildings",
                        principalColumn: "temp_formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_TempFormIOFloors_temp_formiofloor_id",
                        column: x => x.temp_formiofloor_id,
                        principalTable: "TempFormIOFloors",
                        principalColumn: "temp_formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_TempFormIORooms_temp_formioroom_id",
                        column: x => x.temp_formioroom_id,
                        principalTable: "TempFormIORooms",
                        principalColumn: "temp_formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_TempFormIOSections_temp_formiosection_id",
                        column: x => x.temp_formiosection_id,
                        principalTable: "TempFormIOSections",
                        principalColumn: "temp_formiosection_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAsset_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_tempasset_id",
                table: "WOOnboardingAssets",
                column: "tempasset_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_asset_id",
                table: "TempAsset",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_inspectiontemplate_asset_class_id",
                table: "TempAsset",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_site_id",
                table: "TempAsset",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_formiobuilding_id",
                table: "TempAsset",
                column: "temp_formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_formiofloor_id",
                table: "TempAsset",
                column: "temp_formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_formioroom_id",
                table: "TempAsset",
                column: "temp_formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_formiosection_id",
                table: "TempAsset",
                column: "temp_formiosection_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_wo_id",
                table: "TempAsset",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssets_TempAsset_tempasset_id",
                table: "WOOnboardingAssets",
                column: "tempasset_id",
                principalTable: "TempAsset",
                principalColumn: "tempasset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssets_TempAsset_tempasset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropTable(
                name: "TempAsset");

            migrationBuilder.DropIndex(
                name: "IX_WOOnboardingAssets_tempasset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "tempasset_id",
                table: "WOOnboardingAssets");
        }
    }
}
