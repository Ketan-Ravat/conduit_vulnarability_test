using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class formioassetclassadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "asset_class_form_properties",
                table: "InspectionsTemplateFormIO",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "inspectiontemplate_asset_class_id",
                table: "InspectionsTemplateFormIO",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InspectionTemplateAssetClass",
                columns: table => new
                {
                    inspectiontemplate_asset_class_id = table.Column<Guid>(nullable: false),
                    asset_class_code = table.Column<string>(nullable: true),
                    asset_class_name = table.Column<string>(nullable: true),
                    form_type_id = table.Column<int>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    isarchive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspectionTemplateAssetClass", x => x.inspectiontemplate_asset_class_id);
                    table.ForeignKey(
                        name: "FK_InspectionTemplateAssetClass_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InspectionTemplateAssetClass_FormIOType_form_type_id",
                        column: x => x.form_type_id,
                        principalTable: "FormIOType",
                        principalColumn: "form_type_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InspectionsTemplateFormIO_inspectiontemplate_asset_class_id",
                table: "InspectionsTemplateFormIO",
                column: "inspectiontemplate_asset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionTemplateAssetClass_company_id",
                table: "InspectionTemplateAssetClass",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionTemplateAssetClass_form_type_id",
                table: "InspectionTemplateAssetClass",
                column: "form_type_id");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionsTemplateFormIO_InspectionTemplateAssetClass_insp~",
                table: "InspectionsTemplateFormIO",
                column: "inspectiontemplate_asset_class_id",
                principalTable: "InspectionTemplateAssetClass",
                principalColumn: "inspectiontemplate_asset_class_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionsTemplateFormIO_InspectionTemplateAssetClass_insp~",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropTable(
                name: "InspectionTemplateAssetClass");

            migrationBuilder.DropIndex(
                name: "IX_InspectionsTemplateFormIO_inspectiontemplate_asset_class_id",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropColumn(
                name: "asset_class_form_properties",
                table: "InspectionsTemplateFormIO");

            migrationBuilder.DropColumn(
                name: "inspectiontemplate_asset_class_id",
                table: "InspectionsTemplateFormIO");
        }
    }
}
