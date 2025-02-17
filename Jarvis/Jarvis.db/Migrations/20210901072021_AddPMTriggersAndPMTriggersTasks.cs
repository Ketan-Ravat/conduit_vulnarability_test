using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMTriggersAndPMTriggersTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PMTriggers",
                columns: table => new
                {
                    pm_trigger_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    due_datetime = table.Column<DateTime>(nullable: true),
                    due_meter_hours = table.Column<int>(nullable: true),
                    estimated_due_date_meter_hour = table.Column<int>(nullable: true),
                    total_est_time_minutes = table.Column<int>(nullable: true),
                    total_est_time_hours = table.Column<int>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTriggers", x => x.pm_trigger_id);
                    table.ForeignKey(
                        name: "FK_PMTriggers_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggers_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggers_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PMTriggersTasks",
                columns: table => new
                {
                    trigger_task_id = table.Column<Guid>(nullable: false),
                    pm_trigger_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    asset_pm_task_id = table.Column<Guid>(nullable: false),
                    task_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTriggersTasks", x => x.trigger_task_id);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_AssetPMTasks_asset_pm_task_id",
                        column: x => x.asset_pm_task_id,
                        principalTable: "AssetPMTasks",
                        principalColumn: "asset_pm_task_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_PMTriggers_pm_trigger_id",
                        column: x => x.pm_trigger_id,
                        principalTable: "PMTriggers",
                        principalColumn: "pm_trigger_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTriggersTasks_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggers_asset_id",
                table: "PMTriggers",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggers_asset_pm_id",
                table: "PMTriggers",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggers_status",
                table: "PMTriggers",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_asset_id",
                table: "PMTriggersTasks",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_asset_pm_id",
                table: "PMTriggersTasks",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_asset_pm_task_id",
                table: "PMTriggersTasks",
                column: "asset_pm_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_pm_trigger_id",
                table: "PMTriggersTasks",
                column: "pm_trigger_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_status",
                table: "PMTriggersTasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersTasks_task_id",
                table: "PMTriggersTasks",
                column: "task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PMTriggersTasks");

            migrationBuilder.DropTable(
                name: "PMTriggers");
        }
    }
}
