using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMAndPMTasksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PMs",
                columns: table => new
                {
                    pm_id = table.Column<Guid>(nullable: false),
                    title = table.Column<string>(nullable: true),
                    description = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    pm_trigger_type = table.Column<int>(nullable: false),
                    pm_trigger_by = table.Column<int>(nullable: false),
                    datetime_due_at = table.Column<DateTime>(nullable: true),
                    meter_hours_due_at = table.Column<int>(nullable: true),
                    datetime_repeates_every = table.Column<int>(nullable: true),
                    datetime_starting_at = table.Column<DateTime>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    datetime_repeat_time_period_type = table.Column<int>(nullable: true),
                    meter_hours_starting_at = table.Column<int>(nullable: true),
                    meter_hours_repeates_every = table.Column<int>(nullable: true),
                    is_trigger_on_starting_at = table.Column<bool>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMs", x => x.pm_id);
                    table.ForeignKey(
                        name: "FK_PMs_StatusMasters_datetime_repeat_time_period_type",
                        column: x => x.datetime_repeat_time_period_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMs_PMPlans_pm_plan_id",
                        column: x => x.pm_plan_id,
                        principalTable: "PMPlans",
                        principalColumn: "pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMs_StatusMasters_pm_trigger_by",
                        column: x => x.pm_trigger_by,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMs_StatusMasters_pm_trigger_type",
                        column: x => x.pm_trigger_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMs_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PMTasks",
                columns: table => new
                {
                    pm_task_id = table.Column<Guid>(nullable: false),
                    task_id = table.Column<Guid>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: false),
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMTasks", x => x.pm_task_id);
                    table.ForeignKey(
                        name: "FK_PMTasks_PMs_pm_id",
                        column: x => x.pm_id,
                        principalTable: "PMs",
                        principalColumn: "pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTasks_PMPlans_pm_plan_id",
                        column: x => x.pm_plan_id,
                        principalTable: "PMPlans",
                        principalColumn: "pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMTasks_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMs_datetime_repeat_time_period_type",
                table: "PMs",
                column: "datetime_repeat_time_period_type");

            migrationBuilder.CreateIndex(
                name: "IX_PMs_pm_plan_id",
                table: "PMs",
                column: "pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMs_pm_trigger_by",
                table: "PMs",
                column: "pm_trigger_by");

            migrationBuilder.CreateIndex(
                name: "IX_PMs_pm_trigger_type",
                table: "PMs",
                column: "pm_trigger_type");

            migrationBuilder.CreateIndex(
                name: "IX_PMs_status",
                table: "PMs",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_PMTasks_pm_id",
                table: "PMTasks",
                column: "pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTasks_pm_plan_id",
                table: "PMTasks",
                column: "pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMTasks_task_id",
                table: "PMTasks",
                column: "task_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PMTasks");

            migrationBuilder.DropTable(
                name: "PMs");
        }
    }
}
