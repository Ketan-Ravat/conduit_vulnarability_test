using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class BoolKeyAdded_AssetPM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_pm_inspection_manual",
                table: "AssetPMs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_pm_plan_inspection_manual",
                table: "AssetPMPlans",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_pm_inspection_manual",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "is_pm_plan_inspection_manual",
                table: "AssetPMPlans");
        }
    }
}
