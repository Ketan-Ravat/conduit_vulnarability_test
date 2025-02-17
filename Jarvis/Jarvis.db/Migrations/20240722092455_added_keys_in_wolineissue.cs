using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class added_keys_in_wolineissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "nec_violation",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "osha_violation",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_additional_ir_photo",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_location",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_measured_amps",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_measured_temps",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_anomaly_probable_cause",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_anomaly_recommendation",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_refrence_temps",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_sub_componant",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_classification_id",
                table: "WOLineIssue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nec_violation",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "osha_violation",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_additional_ir_photo",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_location",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_measured_amps",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_measured_temps",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_probable_cause",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_recommendation",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_refrence_temps",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_sub_componant",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_classification_id",
                table: "WOLineIssue");
        }
    }
}
