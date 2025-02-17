using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class WorkOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrders",
                columns: table => new
                {
                    wo_id = table.Column<Guid>(nullable: false),
                    wo_number = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    mr_id = table.Column<Guid>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: false),
                    priority = table.Column<int>(nullable: false),
                    due_at = table.Column<DateTime>(nullable: false),
                    wo_type = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    completed_date = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrders", x => x.wo_id);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_MaintenanceRequests_mr_id",
                        column: x => x.mr_id,
                        principalTable: "MaintenanceRequests",
                        principalColumn: "mr_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkOrders_StatusMasters_priority",
                        column: x => x.priority,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrders_StatusMasters_wo_type",
                        column: x => x.wo_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderAttachments",
                columns: table => new
                {
                    wo_attachment_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    user_uploaded_name = table.Column<string>(nullable: true),
                    filename = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderAttachments", x => x.wo_attachment_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderAttachments_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderCancel",
                columns: table => new
                {
                    wo_cancel_id = table.Column<Guid>(nullable: false),
                    reason_id = table.Column<int>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    notes = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderCancel", x => x.wo_cancel_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderCancel_CancelReasonMaster_reason_id",
                        column: x => x.reason_id,
                        principalTable: "CancelReasonMaster",
                        principalColumn: "reason_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderCancel_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkOrderTasks",
                columns: table => new
                {
                    wo_task_id = table.Column<Guid>(nullable: false),
                    task_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    time_spent_minutes = table.Column<int>(nullable: false),
                    time_spent_hours = table.Column<int>(nullable: false),
                    time_remaining_display = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderTasks", x => x.wo_task_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderTasks_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderTasks_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderTasks_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderAttachments_wo_id",
                table: "WorkOrderAttachments",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderCancel_reason_id",
                table: "WorkOrderCancel",
                column: "reason_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderCancel_wo_id",
                table: "WorkOrderCancel",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_asset_id",
                table: "WorkOrders",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_mr_id",
                table: "WorkOrders",
                column: "mr_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_priority",
                table: "WorkOrders",
                column: "priority");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_site_id",
                table: "WorkOrders",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_status",
                table: "WorkOrders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_wo_type",
                table: "WorkOrders",
                column: "wo_type");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_status",
                table: "WorkOrderTasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_task_id",
                table: "WorkOrderTasks",
                column: "task_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderTasks_wo_id",
                table: "WorkOrderTasks",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderAttachments");

            migrationBuilder.DropTable(
                name: "WorkOrderCancel");

            migrationBuilder.DropTable(
                name: "WorkOrderTasks");

            migrationBuilder.DropTable(
                name: "WorkOrders");
        }
    }
}
