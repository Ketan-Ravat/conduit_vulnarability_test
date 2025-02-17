using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ChangeAssetsFKtostring1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets");

            migrationBuilder.AlterColumn<Guid>(
                name: "inspectionform_id",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets",
                column: "inspectionform_id",
                principalTable: "InspectionForms",
                principalColumn: "inspection_form_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets");

            migrationBuilder.AlterColumn<Guid>(
                name: "inspectionform_id",
                table: "Assets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets",
                column: "inspectionform_id",
                principalTable: "InspectionForms",
                principalColumn: "inspection_form_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
