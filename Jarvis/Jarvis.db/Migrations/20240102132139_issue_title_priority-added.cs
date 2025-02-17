using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class issue_title_priorityadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.AddColumn<bool>(
                name: "is_nec_violation_resolved",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_osha_violation_resolved",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_thermal_anomaly_resolved",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);*/

            migrationBuilder.AddColumn<int>(
                name: "issue_priority",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "issue_title",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "new_issue_asset_type",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           /* migrationBuilder.DropColumn(
                name: "is_nec_violation_resolved",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_osha_violation_resolved",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_thermal_anomaly_resolved",
                table: "WOOnboardingAssets");*/

            migrationBuilder.DropColumn(
                name: "issue_priority",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "issue_title",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "new_issue_asset_type",
                table: "WOOnboardingAssets");
        }
    }
}
