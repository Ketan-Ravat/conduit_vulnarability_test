using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class addedformiosection_idtoTempFormIOSections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "formiosection_id",
                table: "TempFormIOSections",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOSections_formiosection_id",
                table: "TempFormIOSections",
                column: "formiosection_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TempFormIOSections_FormIOSections_formiosection_id",
                table: "TempFormIOSections",
                column: "formiosection_id",
                principalTable: "FormIOSections",
                principalColumn: "formiosection_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempFormIOSections_FormIOSections_formiosection_id",
                table: "TempFormIOSections");

            migrationBuilder.DropIndex(
                name: "IX_TempFormIOSections_formiosection_id",
                table: "TempFormIOSections");

            migrationBuilder.DropColumn(
                name: "formiosection_id",
                table: "TempFormIOSections");
        }
    }
}
