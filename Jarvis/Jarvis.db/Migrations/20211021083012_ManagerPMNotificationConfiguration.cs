using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ManagerPMNotificationConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagerPMNotificationConfiguration",
                columns: table => new
                {
                    manager_notification_conf_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    is_disabled = table.Column<bool>(nullable: false),
                    pm_trigger_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagerPMNotificationConfiguration", x => x.manager_notification_conf_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagerPMNotificationConfiguration");
        }
    }
}
