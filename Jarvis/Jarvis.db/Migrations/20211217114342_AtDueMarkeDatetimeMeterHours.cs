using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AtDueMarkeDatetimeMeterHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetPMNotificationConfigurations_asset_id",
                table: "AssetPMNotificationConfigurations");

            migrationBuilder.AddColumn<DateTime>(
                name: "datetime_when_pm_due",
                table: "PMTriggers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "meter_hours_when_pm_due",
                table: "PMTriggers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_asset_id",
                table: "AssetPMNotificationConfigurations",
                column: "asset_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AssetPMNotificationConfigurations_asset_id",
                table: "AssetPMNotificationConfigurations");

            migrationBuilder.DropColumn(
                name: "datetime_when_pm_due",
                table: "PMTriggers");

            migrationBuilder.DropColumn(
                name: "meter_hours_when_pm_due",
                table: "PMTriggers");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_asset_id",
                table: "AssetPMNotificationConfigurations",
                column: "asset_id");
        }
    }
}
