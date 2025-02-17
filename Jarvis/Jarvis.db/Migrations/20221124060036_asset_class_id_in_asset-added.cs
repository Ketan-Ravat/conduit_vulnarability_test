using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_class_id_in_assetadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "inspectiontemplate_asset_class_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_inspectiontemplate_asset_class_id",
                table: "Assets",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_InspectionTemplateAssetClass_inspectiontemplate_asse~",
                table: "Assets",
                column: "inspectiontemplate_asset_class_id",
                principalTable: "InspectionTemplateAssetClass",
                principalColumn: "inspectiontemplate_asset_class_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_InspectionTemplateAssetClass_inspectiontemplate_asse~",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_inspectiontemplate_asset_class_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "inspectiontemplate_asset_class_id",
                table: "Assets");
        }
    }
}
