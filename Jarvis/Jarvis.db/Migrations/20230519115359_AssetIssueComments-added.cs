using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AssetIssueCommentsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetIssueComments",
                columns: table => new
                {
                    asset_issue_comments_id = table.Column<Guid>(nullable: false),
                    asset_issue_id = table.Column<Guid>(nullable: true),
                    comment = table.Column<string>(nullable: true),
                    comment_user_id = table.Column<Guid>(nullable: true),
                    comment_user_role_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetIssueComments", x => x.asset_issue_comments_id);
                    table.ForeignKey(
                        name: "FK_AssetIssueComments_AssetIssue_asset_issue_id",
                        column: x => x.asset_issue_id,
                        principalTable: "AssetIssue",
                        principalColumn: "asset_issue_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssueComments_User_comment_user_id",
                        column: x => x.comment_user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIssueComments_Role_comment_user_role_id",
                        column: x => x.comment_user_role_id,
                        principalTable: "Role",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssueComments_asset_issue_id",
                table: "AssetIssueComments",
                column: "asset_issue_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssueComments_comment_user_id",
                table: "AssetIssueComments",
                column: "comment_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIssueComments_comment_user_role_id",
                table: "AssetIssueComments",
                column: "comment_user_role_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetIssueComments");
        }
    }
}
