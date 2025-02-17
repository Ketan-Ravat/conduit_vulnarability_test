using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class User_AddColumn_ExecutiveProgressReportEmailStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "executive_report_status",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_executive_report_status",
                table: "User",
                column: "executive_report_status");

            migrationBuilder.AddForeignKey(
                name: "FK_User_StatusMasters_executive_report_status",
                table: "User",
                column: "executive_report_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_StatusMasters_executive_report_status",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_executive_report_status",
                table: "User");

            migrationBuilder.DropColumn(
                name: "executive_report_status",
                table: "User");
        }
    }
}
