using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMPlansTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PMPlans",
                columns: table => new
                {
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    plan_name = table.Column<string>(nullable: true),
                    plan_code = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    pm_category_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMPlans", x => x.pm_plan_id);
                    table.ForeignKey(
                        name: "FK_PMPlans_PMCategory_pm_category_id",
                        column: x => x.pm_category_id,
                        principalTable: "PMCategory",
                        principalColumn: "pm_category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMPlans_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMPlans_pm_category_id",
                table: "PMPlans",
                column: "pm_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMPlans_status",
                table: "PMPlans",
                column: "status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PMPlans");
        }
    }
}
