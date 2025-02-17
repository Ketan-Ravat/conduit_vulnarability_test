using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOOnboardingAssetsDateTimeTracking_newTableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WOOnboardingAssetsDateTimeTracking",
                columns: table => new
                {
                    woonboardingassets_datetime_tracking_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    saved_date = table.Column<DateTime>(nullable: true),
                    submitted_date = table.Column<DateTime>(nullable: true),
                    accepted_date = table.Column<DateTime>(nullable: true),
                    rejected_date = table.Column<DateTime>(nullable: true),
                    hold_date = table.Column<DateTime>(nullable: true),
                    deleted_date = table.Column<DateTime>(nullable: true),
                    work_start_date = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOOnboardingAssetsDateTimeTracking", x => x.woonboardingassets_datetime_tracking_id);
                    table.ForeignKey(
                        name: "FK_WOOnboardingAssetsDateTimeTracking_WOOnboardingAssets_woonb~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssetsDateTimeTracking_woonboardingassets_id",
                table: "WOOnboardingAssetsDateTimeTracking",
                column: "woonboardingassets_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOOnboardingAssetsDateTimeTracking");
        }
    }
}
