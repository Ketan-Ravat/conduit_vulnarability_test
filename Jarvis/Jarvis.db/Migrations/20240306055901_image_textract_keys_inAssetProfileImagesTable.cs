using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class image_textract_keys_inAssetProfileImagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "image_actual_json",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_extracted_json",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_actual_json",
                table: "AssetProfileImages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_extracted_json",
                table: "AssetProfileImages",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_actual_json",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropColumn(
                name: "image_extracted_json",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropColumn(
                name: "image_actual_json",
                table: "AssetProfileImages");

            migrationBuilder.DropColumn(
                name: "image_extracted_json",
                table: "AssetProfileImages");
        }
    }
}
