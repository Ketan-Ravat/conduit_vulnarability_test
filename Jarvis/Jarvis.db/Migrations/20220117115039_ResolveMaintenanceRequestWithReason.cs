using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ResolveMaintenanceRequestWithReason : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resolve_reason",
                table: "MaintenanceRequests",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "resolved_at",
                table: "MaintenanceRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resolve_reason",
                table: "MaintenanceRequests");

            migrationBuilder.DropColumn(
                name: "resolved_at",
                table: "MaintenanceRequests");
        }
    }
}
