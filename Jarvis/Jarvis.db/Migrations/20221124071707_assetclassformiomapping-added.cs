using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class assetclassformiomappingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetClassFormIOMapping",
                columns: table => new
                {
                    asset_class_formio_mapping_id = table.Column<Guid>(nullable: false),
                    inspectiontemplate_asset_class_id = table.Column<Guid>(nullable: false),
                    form_id = table.Column<Guid>(nullable: false),
                    isarchive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetClassFormIOMapping", x => x.asset_class_formio_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetClassFormIOMapping_InspectionsTemplateFormIO_form_id",
                        column: x => x.form_id,
                        principalTable: "InspectionsTemplateFormIO",
                        principalColumn: "form_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetClassFormIOMapping_InspectionTemplateAssetClass_inspec~",
                        column: x => x.inspectiontemplate_asset_class_id,
                        principalTable: "InspectionTemplateAssetClass",
                        principalColumn: "inspectiontemplate_asset_class_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetClassFormIOMapping_form_id",
                table: "AssetClassFormIOMapping",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetClassFormIOMapping_inspectiontemplate_asset_class_id",
                table: "AssetClassFormIOMapping",
                column: "inspectiontemplate_asset_class_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetClassFormIOMapping");
        }
    }
}
