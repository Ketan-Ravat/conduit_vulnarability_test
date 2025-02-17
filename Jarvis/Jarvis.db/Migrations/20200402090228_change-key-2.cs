using System;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class changekey2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attribute_id",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderRecord_FormAttributesJsonObject_attrubute_id",
                table: "WorkOrderRecord");

            migrationBuilder.DropTable(
                name: "FormAttributesJsonObject");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_InspectionFormAttributes_attribute_id",
                table: "WorkOrder",
                column: "attribute_id",
                principalTable: "InspectionFormAttributes",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderRecord_InspectionFormAttributes_attrubute_id",
                table: "WorkOrderRecord",
                column: "attrubute_id",
                principalTable: "InspectionFormAttributes",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_InspectionFormAttributes_attribute_id",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrderRecord_InspectionFormAttributes_attrubute_id",
                table: "WorkOrderRecord");

            migrationBuilder.CreateTable(
                name: "FormAttributesJsonObject",
                columns: table => new
                {
                    attributes_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    site_id = table.Column<string>(type: "text", nullable: true),
                    value_parameters = table.Column<AttributeValueJsonObject[]>(type: "jsonb", nullable: true),
                    values_type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAttributesJsonObject", x => x.attributes_id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attribute_id",
                table: "WorkOrder",
                column: "attribute_id",
                principalTable: "FormAttributesJsonObject",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrderRecord_FormAttributesJsonObject_attrubute_id",
                table: "WorkOrderRecord",
                column: "attrubute_id",
                principalTable: "FormAttributesJsonObject",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
