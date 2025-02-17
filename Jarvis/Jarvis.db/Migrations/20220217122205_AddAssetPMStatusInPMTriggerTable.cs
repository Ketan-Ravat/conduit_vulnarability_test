using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetPMStatusInPMTriggerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "asset_pm_status",
                table: "PMTriggers",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggers_asset_pm_status",
                table: "PMTriggers",
                column: "asset_pm_status");

            migrationBuilder.AddForeignKey(
                name: "FK_PMTriggers_StatusMasters_asset_pm_status",
                table: "PMTriggers",
                column: "asset_pm_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PMTriggers_StatusMasters_asset_pm_status",
                table: "PMTriggers");

            migrationBuilder.DropIndex(
                name: "IX_PMTriggers_asset_pm_status",
                table: "PMTriggers");

            migrationBuilder.DropColumn(
                name: "asset_pm_status",
                table: "PMTriggers");
        }
    }
}
