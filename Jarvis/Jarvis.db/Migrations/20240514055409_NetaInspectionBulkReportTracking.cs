using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class NetaInspectionBulkReportTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NetaInspectionBulkReportTracking",
                columns: table => new
                {
                    netainspectionbulkreporttracking_id = table.Column<Guid>(nullable: false),
                    asset_form_ids = table.Column<string>(nullable: true),
                    report_status = table.Column<int>(nullable: false),
                    report_url = table.Column<string>(nullable: true),
                    report_lambda_logs = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_by = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetaInspectionBulkReportTracking", x => x.netainspectionbulkreporttracking_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NetaInspectionBulkReportTracking");
        }
    }
}
