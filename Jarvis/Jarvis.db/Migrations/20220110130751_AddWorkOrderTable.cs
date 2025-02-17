using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddWorkOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_MaintenanceRequests_mr_id",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_mr_id",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "mr_id",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "time_spent_minutes",
                table: "WorkOrderTasks",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "time_spent_hours",
                table: "WorkOrderTasks",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "wo_id",
                table: "MaintenanceRequests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkOrderIssues",
                columns: table => new
                {
                    wo_issue_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    issue_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderIssues", x => x.wo_issue_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderIssues_Issue_issue_id",
                        column: x => x.issue_id,
                        principalTable: "Issue",
                        principalColumn: "issue_uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderIssues_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceRequests_wo_id",
                table: "MaintenanceRequests",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_issue_id",
                table: "WorkOrderIssues",
                column: "issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderIssues_wo_id",
                table: "WorkOrderIssues",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaintenanceRequests_WorkOrders_wo_id",
                table: "MaintenanceRequests",
                column: "wo_id",
                principalTable: "WorkOrders",
                principalColumn: "wo_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaintenanceRequests_WorkOrders_wo_id",
                table: "MaintenanceRequests");

            migrationBuilder.DropTable(
                name: "WorkOrderIssues");

            migrationBuilder.DropIndex(
                name: "IX_MaintenanceRequests_wo_id",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "wo_id",
                table: "MaintenanceRequests");

            migrationBuilder.AlterColumn<int>(
                name: "time_spent_minutes",
                table: "WorkOrderTasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "time_spent_hours",
                table: "WorkOrderTasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "mr_id",
                table: "WorkOrders",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_mr_id",
                table: "WorkOrders",
                column: "mr_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_MaintenanceRequests_mr_id",
                table: "WorkOrders",
                column: "mr_id",
                principalTable: "MaintenanceRequests",
                principalColumn: "mr_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
