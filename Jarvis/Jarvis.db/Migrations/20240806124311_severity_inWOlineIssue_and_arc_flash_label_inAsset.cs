using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class severity_inWOlineIssue_and_arc_flash_label_inAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_corrective_action",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "thermal_anomaly_problem_description",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_anomaly_severity_criteria",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "arc_flash_label_valid",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "arc_flash_label_valid",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thermal_anomaly_corrective_action",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_problem_description",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "thermal_anomaly_severity_criteria",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "arc_flash_label_valid",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "arc_flash_label_valid",
                table: "Assets");
        }
    }
}
