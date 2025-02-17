using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class InspectionsTemplateFormIO_form_type_id_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "form_type_id",
                table: "InspectionsTemplateFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspectionsTemplateFormIO_form_type_id",
                table: "InspectionsTemplateFormIO",
                column: "form_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionsTemplateFormIO_FormIOType_form_type_id",
                table: "InspectionsTemplateFormIO",
                column: "form_type_id",
                principalTable: "FormIOType",
                principalColumn: "form_type_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionsTemplateFormIO_FormIOType_form_type_id",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropIndex(
                name: "IX_InspectionsTemplateFormIO_form_type_id",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropColumn(
                name: "form_type_id",
                table: "InspectionsTemplateFormIO");
        }
    }
}
