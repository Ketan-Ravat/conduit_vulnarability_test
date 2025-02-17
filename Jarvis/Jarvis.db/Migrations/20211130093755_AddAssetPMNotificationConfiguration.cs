using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetPMNotificationConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPMNotificationConfigurations",
                columns: table => new
                {
                    asset_pm_notification_configuration = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
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
                    table.PrimaryKey("PK_AssetPMNotificationConfigurations", x => x.asset_pm_notification_configuration);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_due_at_remi~",
                        column: x => x.due_at_reminder_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_due_at_rem~1",
                        column: x => x.due_at_reminder_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_first_remin~",
                        column: x => x.first_reminder_before_on_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_first_remi~1",
                        column: x => x.first_reminder_before_on_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_first_remi~2",
                        column: x => x.first_reminder_before_on_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_second_remi~",
                        column: x => x.second_reminder_before_on_meter_hours_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_second_rem~1",
                        column: x => x.second_reminder_before_on_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_second_rem~2",
                        column: x => x.second_reminder_before_on_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMNotificationConfigurations_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_asset_id",
                table: "AssetPMNotificationConfigurations",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_due_at_reminder_meter_hou~",
                table: "AssetPMNotificationConfigurations",
                column: "due_at_reminder_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_due_at_reminder_status",
                table: "AssetPMNotificationConfigurations",
                column: "due_at_reminder_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_first_reminder_before_on_~",
                table: "AssetPMNotificationConfigurations",
                column: "first_reminder_before_on_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_first_reminder_before_on~1",
                table: "AssetPMNotificationConfigurations",
                column: "first_reminder_before_on_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_first_reminder_before_on~2",
                table: "AssetPMNotificationConfigurations",
                column: "first_reminder_before_on_type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_second_reminder_before_on~",
                table: "AssetPMNotificationConfigurations",
                column: "second_reminder_before_on_meter_hours_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_second_reminder_before_o~1",
                table: "AssetPMNotificationConfigurations",
                column: "second_reminder_before_on_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_second_reminder_before_o~2",
                table: "AssetPMNotificationConfigurations",
                column: "second_reminder_before_on_type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMNotificationConfigurations_status",
                table: "AssetPMNotificationConfigurations",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetPMNotificationConfigurations");
        }
    }
}
