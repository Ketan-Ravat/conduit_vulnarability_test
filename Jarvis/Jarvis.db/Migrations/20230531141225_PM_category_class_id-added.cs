using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class PM_category_class_idadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "inspectiontemplate_asset_class_id",
                table: "PMCategory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PMCategory_InspectionTemplateAssetClass_inspectiontemplate_~",
                table: "PMCategory",
                column: "inspectiontemplate_asset_class_id",
                principalTable: "InspectionTemplateAssetClass",
                principalColumn: "inspectiontemplate_asset_class_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PMCategory_InspectionTemplateAssetClass_inspectiontemplate_~",
                table: "PMCategory");

            migrationBuilder.DropIndex(
                name: "IX_PMCategory_inspectiontemplate_asset_class_id",
                table: "PMCategory");

            migrationBuilder.DropColumn(
                name: "inspectiontemplate_asset_class_id",
                table: "PMCategory");
        }
    }
}
