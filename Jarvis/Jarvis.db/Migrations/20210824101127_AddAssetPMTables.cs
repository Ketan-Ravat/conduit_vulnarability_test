using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetPMTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPMPlans",
                columns: table => new
                {
                    asset_pm_plan_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    plan_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPMPlans", x => x.asset_pm_plan_id);
                    table.ForeignKey(
                        name: "FK_AssetPMPlans_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMPlans_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetPMs",
                columns: table => new
                {
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: true),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    asset_pm_plan_id = table.Column<Guid>(nullable: false),
                    pm_trigger_type = table.Column<int>(nullable: false),
                    pm_trigger_by = table.Column<int>(nullable: false),
                    datetime_due_at = table.Column<DateTime>(nullable: true),
                    meter_hours_due_at = table.Column<int>(nullable: true),
                    datetime_repeates_every = table.Column<int>(nullable: true),
                    datetime_starting_at = table.Column<DateTime>(nullable: true),
                    datetime_repeat_time_period_type = table.Column<int>(nullable: true),
                    meter_hours_starting_at = table.Column<int>(nullable: true),
                    meter_hours_repeates_every = table.Column<int>(nullable: true),
                    is_trigger_on_starting_at = table.Column<bool>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPMs", x => x.asset_pm_id);
                    table.ForeignKey(
                        name: "FK_AssetPMs_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMs_AssetPMPlans_asset_pm_plan_id",
                        column: x => x.asset_pm_plan_id,
                        principalTable: "AssetPMPlans",
                        principalColumn: "asset_pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMs_StatusMasters_datetime_repeat_time_period_type",
                        column: x => x.datetime_repeat_time_period_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetPMs_StatusMasters_pm_trigger_by",
                        column: x => x.pm_trigger_by,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMs_StatusMasters_pm_trigger_type",
                        column: x => x.pm_trigger_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMs_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetPMTasks",
                columns: table => new
                {
                    asset_pm_task_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    pm_task_id = table.Column<Guid>(nullable: true),
                    task_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    asset_pm_plan_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPMTasks", x => x.asset_pm_task_id);
                    table.ForeignKey(
                        name: "FK_AssetPMTasks_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMTasks_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMTasks_AssetPMPlans_asset_pm_plan_id",
                        column: x => x.asset_pm_plan_id,
                        principalTable: "AssetPMPlans",
                        principalColumn: "asset_pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMTasks_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMPlans_asset_id",
                table: "AssetPMPlans",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMPlans_status",
                table: "AssetPMPlans",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_asset_id",
                table: "AssetPMs",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_asset_pm_plan_id",
                table: "AssetPMs",
                column: "asset_pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_datetime_repeat_time_period_type",
                table: "AssetPMs",
                column: "datetime_repeat_time_period_type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_pm_trigger_by",
                table: "AssetPMs",
                column: "pm_trigger_by");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_pm_trigger_type",
                table: "AssetPMs",
                column: "pm_trigger_type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_status",
                table: "AssetPMs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMTasks_asset_id",
                table: "AssetPMTasks",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMTasks_asset_pm_id",
                table: "AssetPMTasks",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMTasks_asset_pm_plan_id",
                table: "AssetPMTasks",
                column: "asset_pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMTasks_task_id",
                table: "AssetPMTasks",
                column: "task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetPMTasks");

            migrationBuilder.DropTable(
                name: "AssetPMs");

            migrationBuilder.DropTable(
                name: "AssetPMPlans");
        }
    }
}
