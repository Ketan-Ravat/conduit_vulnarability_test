using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class changekey1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attributes_id",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_attributes_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "attributes_id",
                table: "WorkOrder");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_attribute_id",
                table: "WorkOrder",
                column: "attribute_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attribute_id",
                table: "WorkOrder",
                column: "attribute_id",
                principalTable: "FormAttributesJsonObject",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attribute_id",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_attribute_id",
                table: "WorkOrder");

            migrationBuilder.AddColumn<Guid>(
                name: "attributes_id",
                table: "WorkOrder",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_attributes_id",
                table: "WorkOrder",
                column: "attributes_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attributes_id",
                table: "WorkOrder",
                column: "attributes_id",
                principalTable: "FormAttributesJsonObject",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
