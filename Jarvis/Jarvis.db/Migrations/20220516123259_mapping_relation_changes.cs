using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class mapping_relation_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "wo_id",
                unique: true);
        }
    }
}
