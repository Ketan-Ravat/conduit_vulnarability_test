using System;
using System.Collections.Generic;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class RemoveWorkOrdersRelatedModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderRecord");

            migrationBuilder.DropTable(
                name: "WorkOrderStatus");

            migrationBuilder.DropTable(
                name: "WorkOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrder",
                columns: table => new
                {
                    work_order_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attribute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attributes_value = table.Column<AssetsValueJsonObject[]>(type: "jsonb", nullable: true),
                    checkout_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    comments = table.Column<List<CommentJsonObject>>(type: "jsonb", nullable: true),
                    company_id = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    internal_asset_id = table.Column<string>(type: "text", nullable: true),
                    maintainence_staff_id = table.Column<string>(type: "text", nullable: true),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    requested_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    work_order_number = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrder", x => x.work_order_uuid);
                    table.ForeignKey(
                        name: "FK_WorkOrder_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrder_InspectionFormAttributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "InspectionFormAttributes",
                        principalColumn: "attributes_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrder_Inspection_inspection_id",
                        column: x => x.inspection_id,
                        principalTable: "Inspection",
                        principalColumn: "inspection_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrder_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrder_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderRecord",
                columns: table => new
                {
                    work_order_record_uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attrubute_id = table.Column<Guid>(type: "uuid", nullable: false),
                    checkout_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    fixed_by = table.Column<string>(type: "text", nullable: true),
                    fixed_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    inspection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_datetime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    work_order_uuid = table.Column<Guid>(type: "uuid", nullable: false)
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
                        name: "FK_WorkOrderRecord_InspectionFormAttributes_attrubute_id",
                        column: x => x.attrubute_id,
                        principalTable: "InspectionFormAttributes",
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

            migrationBuilder.CreateTable(
                name: "WorkOrderStatus",
                columns: table => new
                {
                    work_order_status_id = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    modified_by = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderStatus", x => x.work_order_status_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderStatus_WorkOrder_work_order_id",
                        column: x => x.work_order_id,
                        principalTable: "WorkOrder",
                        principalColumn: "work_order_uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_asset_id",
                table: "WorkOrder",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_attribute_id",
                table: "WorkOrder",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_inspection_id",
                table: "WorkOrder",
                column: "inspection_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_site_id",
                table: "WorkOrder",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_status",
                table: "WorkOrder",
                column: "status");

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

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderStatus_work_order_id",
                table: "WorkOrderStatus",
                column: "work_order_id");
        }
    }
}
