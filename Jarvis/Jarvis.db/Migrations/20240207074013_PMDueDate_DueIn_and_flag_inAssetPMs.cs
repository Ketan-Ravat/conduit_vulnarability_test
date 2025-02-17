using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class PMDueDate_DueIn_and_flag_inAssetPMs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "due_date",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "pm_due_overdue_flag",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pm_due_time_duration",
                table: "AssetPMs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "due_date",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "pm_due_overdue_flag",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "pm_due_time_duration",
                table: "AssetPMs");
        }
    }
}
