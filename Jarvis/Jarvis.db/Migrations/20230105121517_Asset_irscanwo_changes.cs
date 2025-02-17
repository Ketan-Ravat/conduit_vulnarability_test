using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Asset_irscanwo_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "thermal_classification_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "thermal_classification_id",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "thermal_classification_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "thermal_classification_id",
                table: "Assets");
        }
    }
}
