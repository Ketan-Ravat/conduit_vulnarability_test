using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class photo_type_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "asset_photo_type",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "asset_photo_type",
                table: "AssetProfileImages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_photo_type",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropColumn(
                name: "asset_photo_type",
                table: "AssetProfileImages");
        }
    }
}
