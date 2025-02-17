using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AssetPMsTriggerConditionMappingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPMsTriggerConditionMapping",
                columns: table => new
                {
                    asset_pm_trigger_condition_mapping_id = table.Column<Guid>(nullable: false),
                    datetime_repeates_every = table.Column<int>(nullable: true),
                    datetime_repeat_time_period_type = table.Column<int>(nullable: true),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    condition_type_id = table.Column<int>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPMsTriggerConditionMapping", x => x.asset_pm_trigger_condition_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetPMsTriggerConditionMapping_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMsTriggerConditionMapping_StatusMasters_datetime_repe~",
                        column: x => x.datetime_repeat_time_period_type,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetPMsTriggerConditionMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMsTriggerConditionMapping_asset_pm_id",
                table: "AssetPMsTriggerConditionMapping",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMsTriggerConditionMapping_datetime_repeat_time_period~",
                table: "AssetPMsTriggerConditionMapping",
                column: "datetime_repeat_time_period_type");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMsTriggerConditionMapping_site_id",
                table: "AssetPMsTriggerConditionMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetPMsTriggerConditionMapping");
        }
    }
}
