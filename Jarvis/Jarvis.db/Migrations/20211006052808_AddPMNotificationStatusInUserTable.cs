using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMNotificationStatusInUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "manager_pm_notification_status",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "manager_pm_notification_status",
                table: "User");
        }
    }
}
