using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateWorkOrderTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "hourly_rate",
                table: "WorkOrderTasks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "mr_id",
                table: "Issue",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Issue_mr_id",
                table: "Issue",
                column: "mr_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Issue_MaintenanceRequests_mr_id",
                table: "Issue",
                column: "mr_id",
                principalTable: "MaintenanceRequests",
                principalColumn: "mr_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issue_MaintenanceRequests_mr_id",
                table: "Issue");

            migrationBuilder.DropIndex(
                name: "IX_Issue_mr_id",
                table: "Issue");

            migrationBuilder.DropColumn(
                name: "hourly_rate",
                table: "WorkOrderTasks");

            migrationBuilder.DropColumn(
                name: "mr_id",
                table: "Issue");
        }
    }
}
