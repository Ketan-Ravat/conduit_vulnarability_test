using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_nametemp_issue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "form_retrived_asset_name",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOLineIssue_issue_status",
                table: "WOLineIssue",
                column: "issue_status");

            migrationBuilder.AddForeignKey(
                name: "FK_WOLineIssue_StatusMasters_issue_status",
                table: "WOLineIssue",
                column: "issue_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOLineIssue_StatusMasters_issue_status",
                table: "WOLineIssue");

            migrationBuilder.DropIndex(
                name: "IX_WOLineIssue_issue_status",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "form_retrived_asset_name",
                table: "WOLineIssue");
        }
    }
}
