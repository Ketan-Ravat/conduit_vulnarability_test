using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class wolineissuepm_issue_identity_keyadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pm_issue_identity_key",
                table: "WOLineIssue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pm_issue_identity_key",
                table: "WOLineIssue");
        }
    }
}
