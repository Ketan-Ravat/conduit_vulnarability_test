using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class tempIssueadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WOLineIssue",
                columns: table => new
                {
                    wo_line_issue_id = table.Column<Guid>(nullable: false),
                    issue_type = table.Column<int>(nullable: false),
                    issue_status = table.Column<int>(nullable: true),
                    issue_caused_id = table.Column<int>(nullable: true),
                    asset_form_id = table.Column<Guid>(nullable: true),
                    woonboardingassets_id = table.Column<Guid>(nullable: true),
                    issue_title = table.Column<string>(nullable: true),
                    issue_description = table.Column<string>(nullable: true),
                    field_note = table.Column<string>(nullable: true),
                    back_office_note = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: true),
                    wo_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOLineIssue", x => x.wo_line_issue_id);
                    table.ForeignKey(
                        name: "FK_WOLineIssue_AssetFormIO_asset_form_id",
                        column: x => x.asset_form_id,
                        principalTable: "AssetFormIO",
                        principalColumn: "asset_form_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineIssue_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineIssue_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOLineIssue_WOOnboardingAssets_woonboardingassets_id",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_asset_form_id",
                table: "WOLineIssue",
                column: "asset_form_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_site_id",
                table: "WOLineIssue",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_wo_id",
                table: "WOLineIssue",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_woonboardingassets_id",
                table: "WOLineIssue",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOLineIssue");
        }
    }
}
