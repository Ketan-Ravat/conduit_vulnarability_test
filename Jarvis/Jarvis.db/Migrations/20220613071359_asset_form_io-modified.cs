using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_form_iomodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.AddColumn<string>(
                name: "form_retrived_asset_id",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "form_retrived_asset_name",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "form_retrived_location",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping",
                column: "newly_created_asset_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "form_retrived_asset_id",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "form_retrived_asset_name",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "form_retrived_location",
                table: "AssetFormIO");

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping",
                column: "newly_created_asset_id");
        }
    }
}
