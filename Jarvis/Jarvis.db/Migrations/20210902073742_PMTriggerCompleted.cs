using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class PMTriggerCompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompletedPMTriggers",
                columns: table => new
                {
                    completed_trigger_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    pm_trigger_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    created_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompletedPMTriggers", x => x.completed_trigger_id);
                    table.ForeignKey(
                        name: "FK_CompletedPMTriggers_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompletedPMTriggers_PMTriggers_pm_trigger_id",
                        column: x => x.pm_trigger_id,
                        principalTable: "PMTriggers",
                        principalColumn: "pm_trigger_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PMTriggersRemarks",
                columns: table => new
                {
                    pm_triggers_remark_id = table.Column<Guid>(nullable: false),
                    pm_trigger_id = table.Column<Guid>(nullable: false),
                    comments = table.Column<string>(nullable: true),
                    completed_on = table.Column<DateTime>(nullable: false),
                    completed_at_meter_hours = table.Column<int>(nullable: false),
                    completed_in_hours = table.Column<int>(nullable: false),
                    completed_in_minutes = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    created_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTriggersRemarks", x => x.pm_triggers_remark_id);
                    table.ForeignKey(
                        name: "FK_PMTriggersRemarks_PMTriggers_pm_trigger_id",
                        column: x => x.pm_trigger_id,
                        principalTable: "PMTriggers",
                        principalColumn: "pm_trigger_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompletedPMTriggers_asset_pm_id",
                table: "CompletedPMTriggers",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_CompletedPMTriggers_pm_trigger_id",
                table: "CompletedPMTriggers",
                column: "pm_trigger_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersRemarks_pm_trigger_id",
                table: "PMTriggersRemarks",
                column: "pm_trigger_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompletedPMTriggers");

            migrationBuilder.DropTable(
                name: "PMTriggersRemarks");
        }
    }
}
