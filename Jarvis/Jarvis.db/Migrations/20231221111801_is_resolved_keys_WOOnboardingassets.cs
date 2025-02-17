using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class is_resolved_keys_WOOnboardingassets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
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
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_nec_violation_resolved",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_osha_violation_resolved",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_thermal_anomaly_resolved",
                table: "WOOnboardingAssets");
        }
    }
}
