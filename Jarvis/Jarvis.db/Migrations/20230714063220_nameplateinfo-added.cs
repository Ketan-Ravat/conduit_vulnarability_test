using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class nameplateinfoadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "form_nameplate_info",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "form_nameplate_info",
                table: "InspectionTemplateAssetClass",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "form_nameplate_info",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "form_nameplate_info",
                table: "InspectionTemplateAssetClass");
        }
    }
}
