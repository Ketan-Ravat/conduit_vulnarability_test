using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class obwoasset_IRWO_Field_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "fed_by",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "flag_issue_NEC_Violation",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "flag_issue_OSHA_Violation",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "flag_issue_Thermal_Anamoly_Detected",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "nec_violation",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "osha_violation",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rated_amps",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_location",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_anomaly_probable_cause",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_anomaly_recommendation",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_sub_componant",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "voltage",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IRWOImagesLabelMapping",
                columns: table => new
                {
                    irwoimagelabelmapping_id = table.Column<Guid>(nullable: false),
                    ir_image_label = table.Column<string>(nullable: true),
                    visual_image_label = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    woonboardingassets_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRWOImagesLabelMapping", x => x.irwoimagelabelmapping_id);
                    table.ForeignKey(
                        name: "FK_IRWOImagesLabelMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IRWOImagesLabelMapping_WOOnboardingAssets_woonboardingasset~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRWOImagesLabelMapping_site_id",
                table: "IRWOImagesLabelMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_IRWOImagesLabelMapping_woonboardingassets_id",
                table: "IRWOImagesLabelMapping",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IRWOImagesLabelMapping");

            migrationBuilder.DropColumn(
                name: "fed_by",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "flag_issue_NEC_Violation",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "flag_issue_OSHA_Violation",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "flag_issue_Thermal_Anamoly_Detected",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "nec_violation",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "osha_violation",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "rated_amps",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_location",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_probable_cause",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_recommendation",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_sub_componant",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "voltage",
                table: "WOOnboardingAssets");
        }
    }
}
