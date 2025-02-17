using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class newKeysIn_NetaInspectionBulkReportTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "report_inspection_type",
                table: "NetaInspectionBulkReportTracking",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reprt_completed_date",
                table: "NetaInspectionBulkReportTracking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "report_inspection_type",
                table: "NetaInspectionBulkReportTracking");

            migrationBuilder.DropColumn(
                name: "reprt_completed_date",
                table: "NetaInspectionBulkReportTracking");
        }
    }
}
