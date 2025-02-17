using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class thumbnail_image_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "asset_thumbnail_photo",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "asset_thumbnail_photo",
                table: "AssetProfileImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_thumbnail_photo",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropColumn(
                name: "asset_thumbnail_photo",
                table: "AssetProfileImages");
        }
    }
}
