using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class obwoasset_IRWO_Field_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "manufacturer",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "model",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "manufacturer",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "model",
                table: "WOOnboardingAssets");
        }
    }
}
