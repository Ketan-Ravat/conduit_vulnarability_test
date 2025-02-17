using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class site_id_added_NetaInsp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "site_id",
                table: "NetaInspectionBulkReportTracking",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetaInspectionBulkReportTracking_site_id",
                table: "NetaInspectionBulkReportTracking",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_NetaInspectionBulkReportTracking_Sites_site_id",
                table: "NetaInspectionBulkReportTracking",
                column: "site_id",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NetaInspectionBulkReportTracking_Sites_site_id",
                table: "NetaInspectionBulkReportTracking");

            migrationBuilder.DropIndex(
                name: "IX_NetaInspectionBulkReportTracking_site_id",
                table: "NetaInspectionBulkReportTracking");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "NetaInspectionBulkReportTracking");
        }
    }
}
