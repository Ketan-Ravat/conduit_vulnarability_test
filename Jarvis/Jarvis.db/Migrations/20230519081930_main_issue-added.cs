using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class main_issueadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_main_issue_created",
                table: "WOLineIssue",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AssetIssue",
                columns: table => new
                {
                    asset_issue_id = table.Column<Guid>(nullable: false),
                    issue_type = table.Column<int>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true),
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
                    wo_line_issue_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetIssue", x => x.asset_issue_id);
                    table.ForeignKey(
                        name: "FK_AssetIssue_AssetFormIO_asset_form_id",
                        column: x => x.asset_form_id,
                        principalTable: "AssetFormIO",
                        principalColumn: "asset_form_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_StatusMasters_issue_status",
                        column: x => x.issue_status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_WOLineIssue_wo_line_issue_id",
                        column: x => x.wo_line_issue_id,
                        principalTable: "WOLineIssue",
                        principalColumn: "wo_line_issue_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssue_WOOnboardingAssets_woonboardingassets_id",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_asset_id",
                table: "WOLineIssue",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_asset_form_id",
                table: "AssetIssue",
                column: "asset_form_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_asset_id",
                table: "AssetIssue",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_issue_status",
                table: "AssetIssue",
                column: "issue_status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_site_id",
                table: "AssetIssue",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_wo_id",
                table: "AssetIssue",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_wo_line_issue_id",
                table: "AssetIssue",
                column: "wo_line_issue_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssue_woonboardingassets_id",
                table: "AssetIssue",
                column: "woonboardingassets_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOLineIssue_Assets_asset_id",
                table: "WOLineIssue",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOLineIssue_Assets_asset_id",
                table: "WOLineIssue");

            migrationBuilder.DropTable(
                name: "AssetIssue");

            migrationBuilder.DropIndex(
                name: "IX_WOLineIssue_asset_id",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "is_main_issue_created",
                table: "WOLineIssue");
        }
    }
}
