using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class mapping_changes_v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_task_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_task_id",
                table: "WOcategorytoTaskMapping",
                column: "task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_task_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_task_id",
                table: "WOcategorytoTaskMapping",
                column: "task_id",
                unique: true);
        }
    }
}
