using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetMeterHoursHistoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetMeterHourHistory",
                columns: table => new
                {
                    asset_meter_hour_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    meter_hours = table.Column<long>(nullable: true),
                    requested_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    company_id = table.Column<string>(nullable: true),
                    site_id = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetMeterHourHistory", x => x.asset_meter_hour_id);
                    table.ForeignKey(
                        name: "FK_AssetMeterHourHistory_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetMeterHourHistory_asset_id",
                table: "AssetMeterHourHistory",
                column: "asset_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetMeterHourHistory");
        }
    }
}
