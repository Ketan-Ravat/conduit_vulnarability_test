using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class pdf_report_added_WOs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ir_wo_export_pdf_at",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ir_wo_pdf_report",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ir_wo_pdf_report_status",
                table: "WorkOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ir_wo_export_pdf_at",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ir_wo_pdf_report",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ir_wo_pdf_report_status",
                table: "WorkOrders");
        }
    }
}
