using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateWorkorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdAt",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "modifiedAt",
                table: "WorkOrder");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "WorkOrder",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "WorkOrder",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "WorkOrder");

            migrationBuilder.AddColumn<DateTime>(
                name: "createdAt",
                table: "WorkOrder",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modifiedAt",
                table: "WorkOrder",
                type: "timestamp without time zone",
                nullable: true);
        }
    }
}
