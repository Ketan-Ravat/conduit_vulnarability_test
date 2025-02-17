using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class formioreportchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "accepted_at",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "export_pdf_at",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "inspected_at",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pdf_report_status",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pdf_report_url",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_pdf_report_status",
                table: "AssetFormIO",
                column: "pdf_report_status");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO",
                column: "pdf_report_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_StatusMasters_pdf_report_status",
                table: "AssetFormIO");

            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_pdf_report_status",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "accepted_at",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "export_pdf_at",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "inspected_at",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "pdf_report_status",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "pdf_report_url",
                table: "AssetFormIO");
        }
    }
}
