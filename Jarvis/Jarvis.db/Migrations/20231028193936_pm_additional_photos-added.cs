using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class pm_additional_photosadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.AddColumn<DateTime>(
                name: "completed_at",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "inspected_at",
                table: "WOOnboardingAssets",
                nullable: true);*/

            migrationBuilder.AddColumn<string>(
                name: "pm_photo_caption",
                table: "AssetProfileImages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WOlineAssetPMImagesMapping",
                columns: table => new
                {
                    woline_assetpm_images_mapping_id = table.Column<Guid>(nullable: false),
                    image_name = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    image_type = table.Column<int>(nullable: false),
                    pm_image_caption = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    active_asset_pm_woline_mapping_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineAssetPMImagesMapping", x => x.woline_assetpm_images_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineAssetPMImagesMapping_ActiveAssetPMWOlineMapping_activ~",
                        column: x => x.active_asset_pm_woline_mapping_id,
                        principalTable: "ActiveAssetPMWOlineMapping",
                        principalColumn: "active_asset_pm_woline_mapping_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOlineAssetPMImagesMapping_active_asset_pm_woline_mapping_id",
                table: "WOlineAssetPMImagesMapping",
                column: "active_asset_pm_woline_mapping_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOlineAssetPMImagesMapping");

           /* migrationBuilder.DropColumn(
                name: "completed_at",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "inspected_at",
                table: "WOOnboardingAssets");
            */
            migrationBuilder.DropColumn(
                name: "pm_photo_caption",
                table: "AssetProfileImages");
        }
    }
}
