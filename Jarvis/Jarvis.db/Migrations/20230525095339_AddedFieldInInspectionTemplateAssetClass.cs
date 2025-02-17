using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddedFieldInInspectionTemplateAssetClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "asset_expected_usefull_life",
                table: "InspectionTemplateAssetClass",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "asset_expected_usefull_life",
                table: "InspectionTemplateAssetClass");
        }
    }
}
