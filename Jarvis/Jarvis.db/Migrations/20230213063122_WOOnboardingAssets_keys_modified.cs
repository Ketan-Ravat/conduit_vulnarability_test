using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOOnboardingAssets_keys_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                table: "WOOnboardingAssets");

            migrationBuilder.RenameColumn(
                name: "flag_issue_Thermal_Anamoly_Detected",
                table: "WOOnboardingAssets",
                newName: "flag_issue_thermal_anamoly_detected");

            migrationBuilder.RenameColumn(
                name: "flag_issue_OSHA_Violation",
                table: "WOOnboardingAssets",
                newName: "flag_issue_osha_violation");

            migrationBuilder.RenameColumn(
                name: "flag_issue_NEC_Violation",
                table: "WOOnboardingAssets",
                newName: "flag_issue_nec_violation");

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_additional_ir_photo",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_measured_amps",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_measured_temps",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_refrence_temps",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thermal_anomaly_additional_ir_photo",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_measured_amps",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_measured_temps",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_refrence_temps",
                table: "WOOnboardingAssets");

            migrationBuilder.RenameColumn(
                name: "flag_issue_thermal_anamoly_detected",
                table: "WOOnboardingAssets",
                newName: "flag_issue_Thermal_Anamoly_Detected");

            migrationBuilder.RenameColumn(
                name: "flag_issue_osha_violation",
                table: "WOOnboardingAssets",
                newName: "flag_issue_OSHA_Violation");

            migrationBuilder.RenameColumn(
                name: "flag_issue_nec_violation",
                table: "WOOnboardingAssets",
                newName: "flag_issue_NEC_Violation");

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "WOOnboardingAssets",
                type: "text",
                nullable: true);
        }
    }
}
