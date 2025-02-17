using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class InspectionsTemplateFormIO_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_form_id",
                table: "Assets");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_form_id",
                table: "Assets",
                column: "form_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assets_form_id",
                table: "Assets");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_form_id",
                table: "Assets",
                column: "form_id",
                unique: true);
        }
    }
}
