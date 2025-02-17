using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ActiveAssetPMWOlineMappingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActiveAssetPMWOlineMapping",
                columns: table => new
                {
                    active_asset_pm_woline_mapping_id = table.Column<Guid>(nullable: false),
                    is_active = table.Column<bool>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    pm_form_output_data = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveAssetPMWOlineMapping", x => x.active_asset_pm_woline_mapping_id);
                    table.ForeignKey(
                        name: "FK_ActiveAssetPMWOlineMapping_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActiveAssetPMWOlineMapping_WOOnboardingAssets_woonboardinga~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

          /*  migrationBuilder.CreateTable(
                name: "WOlineIssueImagesMapping",
                columns: table => new
                {
                    woline_issue_image_mapping_id = table.Column<Guid>(nullable: false),
                    wo_line_issue_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    image_file_name = table.Column<string>(nullable: true),
                    image_thumbnail_file_name = table.Column<string>(nullable: true),
                    image_duration_type_id = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineIssueImagesMapping", x => x.woline_issue_image_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineIssueImagesMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOlineIssueImagesMapping_WOLineIssue_wo_line_issue_id",
                        column: x => x.wo_line_issue_id,
                        principalTable: "WOLineIssue",
                        principalColumn: "wo_line_issue_id",
                        onDelete: ReferentialAction.Cascade);
                });*/

            migrationBuilder.CreateIndex(
                name: "IX_ActiveAssetPMWOlineMapping_asset_pm_id",
                table: "ActiveAssetPMWOlineMapping",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_ActiveAssetPMWOlineMapping_woonboardingassets_id",
                table: "ActiveAssetPMWOlineMapping",
                column: "woonboardingassets_id",
                unique: true);

          /*  migrationBuilder.CreateIndex(
                name: "IX_WOlineIssueImagesMapping_site_id",
                table: "WOlineIssueImagesMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineIssueImagesMapping_wo_line_issue_id",
                table: "WOlineIssueImagesMapping",
                column: "wo_line_issue_id");*/
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveAssetPMWOlineMapping");

          //  migrationBuilder.DropTable(
          //      name: "WOlineIssueImagesMapping");
        }
    }
}
