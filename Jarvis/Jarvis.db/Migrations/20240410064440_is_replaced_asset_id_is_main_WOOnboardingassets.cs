using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class is_replaced_asset_id_is_main_WOOnboardingassets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_replaced_asset_id_is_main",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_replaced_asset_id_is_main",
                table: "WOOnboardingAssets");
        }
    }
}
