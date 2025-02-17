using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class PMNotificationChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PMTriggersRemarks_pm_trigger_id",
                table: "PMTriggersRemarks");

            migrationBuilder.CreateTable(
                name: "CompanyPMNotificationConfigurations",
                columns: table => new
                {
                    company_pm_notification_configuration = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    first_reminder_before_on = table.Column<int>(nullable: false),
                    first_reminder_before_on_type = table.Column<int>(nullable: false),
                    first_reminder_before_on_status = table.Column<int>(nullable: false),
                    second_reminder_before_on = table.Column<int>(nullable: false),
                    second_reminder_before_on_type = table.Column<int>(nullable: false),
                    second_reminder_before_on_status = table.Column<int>(nullable: false),
                    due_at_reminder_status = table.Column<int>(nullable: false),
                    first_reminder_before_on_meter_hours = table.Column<int>(nullable: false),
                    first_reminder_before_on_meter_hours_status = table.Column<int>(nullable: false),
                    second_reminder_before_on_meter_hours = table.Column<int>(nullable: false),
                    second_reminder_before_on_meter_hours_status = table.Column<int>(nullable: false),
                    due_at_reminder_meter_hours_status = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPMNotificationConfigurations", x => x.company_pm_notification_configuration);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_due_at_re~",
                        column: x => x.due_at_reminder_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_due_at_r~1",
                        column: x => x.due_at_reminder_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_first_rem~",
                        column: x => x.first_reminder_before_on_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_first_re~1",
                        column: x => x.first_reminder_before_on_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_first_re~2",
                        column: x => x.first_reminder_before_on_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_second_re~",
                        column: x => x.second_reminder_before_on_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_second_r~1",
                        column: x => x.second_reminder_before_on_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_second_r~2",
                        column: x => x.second_reminder_before_on_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPMNotificationConfigurations_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersRemarks_pm_trigger_id",
                table: "PMTriggersRemarks",
                column: "pm_trigger_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_company_id",
                table: "CompanyPMNotificationConfigurations",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_due_at_reminder_meter_h~",
                table: "CompanyPMNotificationConfigurations",
                column: "due_at_reminder_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_due_at_reminder_status",
                table: "CompanyPMNotificationConfigurations",
                column: "due_at_reminder_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_first_reminder_before_o~",
                table: "CompanyPMNotificationConfigurations",
                column: "first_reminder_before_on_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_first_reminder_before_~1",
                table: "CompanyPMNotificationConfigurations",
                column: "first_reminder_before_on_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_first_reminder_before_~2",
                table: "CompanyPMNotificationConfigurations",
                column: "first_reminder_before_on_type");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_second_reminder_before_~",
                table: "CompanyPMNotificationConfigurations",
                column: "second_reminder_before_on_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_second_reminder_before~1",
                table: "CompanyPMNotificationConfigurations",
                column: "second_reminder_before_on_status");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_second_reminder_before~2",
                table: "CompanyPMNotificationConfigurations",
                column: "second_reminder_before_on_type");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPMNotificationConfigurations_status",
                table: "CompanyPMNotificationConfigurations",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyPMNotificationConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_PMTriggersRemarks_pm_trigger_id",
                table: "PMTriggersRemarks");

            migrationBuilder.CreateIndex(
                name: "IX_PMTriggersRemarks_pm_trigger_id",
                table: "PMTriggersRemarks",
                column: "pm_trigger_id");
        }
    }
}
