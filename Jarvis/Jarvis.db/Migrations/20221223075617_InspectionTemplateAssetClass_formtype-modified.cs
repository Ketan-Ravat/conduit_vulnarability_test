using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class InspectionTemplateAssetClass_formtypemodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionTemplateAssetClass_FormIOType_form_type_id",
                table: "InspectionTemplateAssetClass");

            migrationBuilder.AlterColumn<int>(
                name: "form_type_id",
                table: "InspectionTemplateAssetClass",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionTemplateAssetClass_FormIOType_form_type_id",
                table: "InspectionTemplateAssetClass",
                column: "form_type_id",
                principalTable: "FormIOType",
                principalColumn: "form_type_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionTemplateAssetClass_FormIOType_form_type_id",
                table: "InspectionTemplateAssetClass");

            migrationBuilder.AlterColumn<int>(
                name: "form_type_id",
                table: "InspectionTemplateAssetClass",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionTemplateAssetClass_FormIOType_form_type_id",
                table: "InspectionTemplateAssetClass",
                column: "form_type_id",
                principalTable: "FormIOType",
                principalColumn: "form_type_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
