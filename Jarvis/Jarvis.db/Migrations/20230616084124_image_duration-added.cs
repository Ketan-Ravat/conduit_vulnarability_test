using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class image_durationadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "image_duration_type_id",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "image_duration_type_id",
                table: "WOOnboardingAssetsImagesMapping");
        }
    }
}
