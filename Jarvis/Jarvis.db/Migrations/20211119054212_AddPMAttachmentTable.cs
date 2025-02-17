using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMAttachmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetPMAttachments",
                columns: table => new
                {
                    asset_pm_attachment_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    asset_pm_id = table.Column<Guid>(nullable: false),
                    asset_pm_plan_id = table.Column<Guid>(nullable: false),
                    user_uploaded_name = table.Column<string>(nullable: true),
                    filename = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPMAttachments", x => x.asset_pm_attachment_id);
                    table.ForeignKey(
                        name: "FK_AssetPMAttachments_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMAttachments_AssetPMs_asset_pm_id",
                        column: x => x.asset_pm_id,
                        principalTable: "AssetPMs",
                        principalColumn: "asset_pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetPMAttachments_AssetPMPlans_asset_pm_plan_id",
                        column: x => x.asset_pm_plan_id,
                        principalTable: "AssetPMPlans",
                        principalColumn: "asset_pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PMAttachments",
                columns: table => new
                {
                    pm_attachment_id = table.Column<Guid>(nullable: false),
                    pm_id = table.Column<Guid>(nullable: false),
                    pm_plan_id = table.Column<Guid>(nullable: false),
                    user_uploaded_name = table.Column<string>(nullable: true),
                    filename = table.Column<string>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMAttachments", x => x.pm_attachment_id);
                    table.ForeignKey(
                        name: "FK_PMAttachments_PMs_pm_id",
                        column: x => x.pm_id,
                        principalTable: "PMs",
                        principalColumn: "pm_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PMAttachments_PMPlans_pm_plan_id",
                        column: x => x.pm_plan_id,
                        principalTable: "PMPlans",
                        principalColumn: "pm_plan_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMAttachments_asset_id",
                table: "AssetPMAttachments",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMAttachments_asset_pm_id",
                table: "AssetPMAttachments",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMAttachments_asset_pm_plan_id",
                table: "AssetPMAttachments",
                column: "asset_pm_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMAttachments_pm_id",
                table: "PMAttachments",
                column: "pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMAttachments_pm_plan_id",
                table: "PMAttachments",
                column: "pm_plan_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetPMAttachments");

            migrationBuilder.DropTable(
                name: "PMAttachments");
        }
    }
}
