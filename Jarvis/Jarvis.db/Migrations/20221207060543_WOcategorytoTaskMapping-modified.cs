using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOcategorytoTaskMappingmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOcategorytoTaskMapping_Tasks_task_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.AddColumn<Guid>(
                name: "inspectiontemplate_asset_class_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "task_id",
                table: "WOcategorytoTaskMapping",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_inspectiontemplate_as~",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOcategorytoTaskMapping_Tasks_task_id",
                table: "WOcategorytoTaskMapping",
                column: "task_id",
                principalTable: "Tasks",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WOInspectionsTemplateFormIOAssignment_InspectionTemplateAss~",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "inspectiontemplate_asset_class_id",
                principalTable: "InspectionTemplateAssetClass",
                principalColumn: "inspectiontemplate_asset_class_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOcategorytoTaskMapping_Tasks_task_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_WOInspectionsTemplateFormIOAssignment_InspectionTemplateAss~",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_inspectiontemplate_as~",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.DropColumn(
                name: "inspectiontemplate_asset_class_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.AlterColumn<Guid>(
                name: "task_id",
                table: "WOcategorytoTaskMapping",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WOcategorytoTaskMapping_Tasks_task_id",
                table: "WOcategorytoTaskMapping",
                column: "task_id",
                principalTable: "Tasks",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
