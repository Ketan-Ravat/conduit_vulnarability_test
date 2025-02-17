using System;
using System.Collections.Generic;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class addworkorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "attribute_id",
                table: "WorkOrder",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "attributes_id",
                table: "WorkOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "checkout_datetime",
                table: "WorkOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "requested_datetime",
                table: "WorkOrder",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            //migrationBuilder.CreateTable(
            //    name: "DashboardOutstandingIssues",
            //    columns: table => new
            //    {
            //        site_id = table.Column<Guid>(nullable: false),
            //        data = table.Column<List<ReportJsonData>>(type: "jsonb", nullable: true),
            //        created_at = table.Column<DateTime>(nullable: false),
            //        modified_at = table.Column<DateTime>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_DashboardOutstandingIssues", x => x.site_id);
            //    });

            migrationBuilder.CreateTable(
                name: "FormAttributesJsonObject",
                columns: table => new
                {
                    attributes_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    values_type = table.Column<int>(nullable: false),
                    company_id = table.Column<string>(nullable: true),
                    category_id = table.Column<int>(nullable: false),
                    site_id = table.Column<string>(nullable: true),
                    value_parameters = table.Column<AttributeValueJsonObject[]>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormAttributesJsonObject", x => x.attributes_id);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderRecord",
                columns: table => new
                {
                    work_order_record_uuid = table.Column<Guid>(nullable: false),
                    work_order_uuid = table.Column<Guid>(nullable: false),
                    attrubute_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    inspection_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    requested_datetime = table.Column<DateTime>(nullable: false),
                    created_datetime = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<Guid>(nullable: false),
                    fixed_datetime = table.Column<DateTime>(nullable: true),
                    fixed_by = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderRecord", x => x.work_order_record_uuid);
                    table.ForeignKey(
                        name: "FK_WorkOrderRecord_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderRecord_FormAttributesJsonObject_attrubute_id",
                        column: x => x.attrubute_id,
                        principalTable: "FormAttributesJsonObject",
                        principalColumn: "attributes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderRecord_Inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspection",
                        principalColumn: "inspection_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderRecord_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderRecord_WorkOrder_work_order_uuid",
                        column: x => x.work_order_uuid,
                        principalTable: "WorkOrder",
                        principalColumn: "work_order_uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_asset_id",
                table: "WorkOrder",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_attributes_id",
                table: "WorkOrder",
                column: "attributes_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRecord_asset_id",
                table: "WorkOrderRecord",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRecord_attrubute_id",
                table: "WorkOrderRecord",
                column: "attrubute_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRecord_inspection_id",
                table: "WorkOrderRecord",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRecord_status",
                table: "WorkOrderRecord",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderRecord_work_order_uuid",
                table: "WorkOrderRecord",
                column: "work_order_uuid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attributes_id",
                table: "WorkOrder",
                column: "attributes_id",
                principalTable: "FormAttributesJsonObject",
                principalColumn: "attributes_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_FormAttributesJsonObject_attributes_id",
                table: "WorkOrder");

            migrationBuilder.DropTable(
                name: "DashboardOutstandingIssues");

            migrationBuilder.DropTable(
                name: "WorkOrderRecord");

            migrationBuilder.DropTable(
                name: "FormAttributesJsonObject");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_asset_id",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_attributes_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "attribute_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "attributes_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "checkout_datetime",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "requested_datetime",
                table: "WorkOrder");
        }
    }
}
