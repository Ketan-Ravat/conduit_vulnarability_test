using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class wocategorytaskmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.AddColumn<int>(
                name: "form_type_id",
                table: "InspectionForms",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping",
                column: "assigned_asset");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_form_type_id",
                table: "InspectionForms",
                column: "form_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionForms_FormIOType_form_type_id",
                table: "InspectionForms",
                column: "form_type_id",
                principalTable: "FormIOType",
                principalColumn: "form_type_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionForms_FormIOType_form_type_id",
                table: "InspectionForms");

            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropIndex(
                name: "IX_InspectionForms_form_type_id",
                table: "InspectionForms");

            migrationBuilder.DropColumn(
                name: "form_type_id",
                table: "InspectionForms");

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping",
                column: "assigned_asset",
                unique: true);
        }
    }
}
