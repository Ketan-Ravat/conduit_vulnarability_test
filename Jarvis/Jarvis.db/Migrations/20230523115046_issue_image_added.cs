using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class issue_image_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetIssueImagesMapping",
                columns: table => new
                {
                    asset_issue_image_mapping_id = table.Column<Guid>(nullable: false),
                    asset_issue_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    image_file_name = table.Column<string>(nullable: true),
                    image_thumbnail_file_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetIssueImagesMapping", x => x.asset_issue_image_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetIssueImagesMapping_AssetIssue_asset_issue_id",
                        column: x => x.asset_issue_id,
                        principalTable: "AssetIssue",
                        principalColumn: "asset_issue_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetIssueImagesMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssueImagesMapping_asset_issue_id",
                table: "AssetIssueImagesMapping",
                column: "asset_issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssueImagesMapping_site_id",
                table: "AssetIssueImagesMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetIssueImagesMapping");
        }
    }
}
