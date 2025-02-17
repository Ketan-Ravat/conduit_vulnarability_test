using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class pmtriggerconditionadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PMsTriggerConditionMapping",
                columns: table => new
                {
                    pm_trigger_condition_mapping_id = table.Column<Guid>(nullable: false),
                    datetime_repeates_every = table.Column<int>(nullable: true),
                    datetime_repeat_time_period_type = table.Column<int>(nullable: true),
                    condition_type_id = table.Column<int>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMsTriggerConditionMapping", x => x.pm_trigger_condition_mapping_id);
                    table.ForeignKey(
                        name: "FK_PMsTriggerConditionMapping_StatusMasters_datetime_repeat_ti~",
                        column: x => x.datetime_repeat_time_period_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PMsTriggerConditionMapping_PMs_pm_id",
                        column: x => x.pm_id,
                        principalTable: "PMs",
                        principalColumn: "pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMsTriggerConditionMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMsTriggerConditionMapping_datetime_repeat_time_period_type",
                table: "PMsTriggerConditionMapping",
                column: "datetime_repeat_time_period_type");

            migrationBuilder.CreateIndex(
                name: "IX_PMsTriggerConditionMapping_pm_id",
                table: "PMsTriggerConditionMapping",
                column: "pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMsTriggerConditionMapping_site_id",
                table: "PMsTriggerConditionMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PMsTriggerConditionMapping");
        }
    }
}
