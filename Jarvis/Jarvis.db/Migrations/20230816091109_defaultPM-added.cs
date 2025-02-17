using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class defaultPMadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory");

            migrationBuilder.AddColumn<bool>(
                name: "is_default_pm_plan",
                table: "PMPlans",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory",
                column: "inspectiontemplate_asset_class_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory");

            migrationBuilder.DropColumn(
                name: "is_default_pm_plan",
                table: "PMPlans");

            migrationBuilder.CreateIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory",
                column: "inspectiontemplate_asset_class_id");
        }
    }
}
