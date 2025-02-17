using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TempAssetpmTablesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_ActiveAssetPMWOlineMapping_activ~",
                table: "WOlineAssetPMImagesMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "active_asset_pm_woline_mapping_id",
                table: "WOlineAssetPMImagesMapping",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "temp_active_asset_pm_woline_mapping_id",
                table: "WOlineAssetPMImagesMapping",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TempAssetPMs",
                columns: table => new
                {
                    temp_asset_pm_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    is_Asset_PM_fixed = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempAssetPMs", x => x.temp_asset_pm_id);
                    table.ForeignKey(
                        name: "FK_TempAssetPMs_PMs_pm_id",
                        column: x => x.pm_id,
                        principalTable: "PMs",
                        principalColumn: "pm_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempAssetPMs_WOOnboardingAssets_woonboardingassets_id",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempActiveAssetPMWOlineMapping",
                columns: table => new
                {
                    temp_active_asset_pm_woline_mapping_id = table.Column<Guid>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    temp_asset_pm_id = table.Column<Guid>(nullable: false),
                    pm_form_output_data = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempActiveAssetPMWOlineMapping", x => x.temp_active_asset_pm_woline_mapping_id);
                    table.ForeignKey(
                        name: "FK_TempActiveAssetPMWOlineMapping_TempAssetPMs_temp_asset_pm_id",
                        column: x => x.temp_asset_pm_id,
                        principalTable: "TempAssetPMs",
                        principalColumn: "temp_asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempActiveAssetPMWOlineMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOlineAssetPMImagesMapping_temp_active_asset_pm_woline_mapp~",
                table: "WOlineAssetPMImagesMapping",
                column: "temp_active_asset_pm_woline_mapping_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempActiveAssetPMWOlineMapping_temp_asset_pm_id",
                table: "TempActiveAssetPMWOlineMapping",
                column: "temp_asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempActiveAssetPMWOlineMapping_woonboardingassets_id",
                table: "TempActiveAssetPMWOlineMapping",
                column: "woonboardingassets_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TempAssetPMs_pm_id",
                table: "TempAssetPMs",
                column: "pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAssetPMs_woonboardingassets_id",
                table: "TempAssetPMs",
                column: "woonboardingassets_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_ActiveAssetPMWOlineMapping_activ~",
                table: "WOlineAssetPMImagesMapping",
                column: "active_asset_pm_woline_mapping_id",
                principalTable: "ActiveAssetPMWOlineMapping",
                principalColumn: "active_asset_pm_woline_mapping_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_TempActiveAssetPMWOlineMapping_t~",
                table: "WOlineAssetPMImagesMapping",
                column: "temp_active_asset_pm_woline_mapping_id",
                principalTable: "TempActiveAssetPMWOlineMapping",
                principalColumn: "temp_active_asset_pm_woline_mapping_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_ActiveAssetPMWOlineMapping_activ~",
                table: "WOlineAssetPMImagesMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_TempActiveAssetPMWOlineMapping_t~",
                table: "WOlineAssetPMImagesMapping");

            migrationBuilder.DropTable(
                name: "TempActiveAssetPMWOlineMapping");

            migrationBuilder.DropTable(
                name: "TempAssetPMs");

            migrationBuilder.DropIndex(
                name: "IX_WOlineAssetPMImagesMapping_temp_active_asset_pm_woline_mapp~",
                table: "WOlineAssetPMImagesMapping");

            migrationBuilder.DropColumn(
                name: "temp_active_asset_pm_woline_mapping_id",
                table: "WOlineAssetPMImagesMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "active_asset_pm_woline_mapping_id",
                table: "WOlineAssetPMImagesMapping",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WOlineAssetPMImagesMapping_ActiveAssetPMWOlineMapping_activ~",
                table: "WOlineAssetPMImagesMapping",
                column: "active_asset_pm_woline_mapping_id",
                principalTable: "ActiveAssetPMWOlineMapping",
                principalColumn: "active_asset_pm_woline_mapping_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
