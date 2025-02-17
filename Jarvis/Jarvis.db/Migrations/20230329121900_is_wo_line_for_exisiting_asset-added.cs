using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class is_wo_line_for_exisiting_assetadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_wo_line_for_exisiting_asset",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_wo_line_for_exisiting_asset",
                table: "WOOnboardingAssets");
        }
    }
}
