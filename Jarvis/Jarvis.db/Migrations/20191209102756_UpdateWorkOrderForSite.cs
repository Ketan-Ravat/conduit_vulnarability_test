using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateWorkOrderForSite : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "company_id",
                table: "WorkOrder",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "site_id",
                table: "WorkOrder",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modified_by",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_site_id",
                table: "WorkOrder",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Sites_site_id",
                table: "WorkOrder",
                column: "site_id",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Sites_site_id",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_site_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "Assets");
        }
    }
}
