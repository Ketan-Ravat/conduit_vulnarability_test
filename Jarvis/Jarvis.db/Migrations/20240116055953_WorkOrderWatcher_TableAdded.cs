using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WorkOrderWatcher_TableAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "notification_user_role",
                table: "NotificationData",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkOrderWatcherUserMapping",
                columns: table => new
                {
                    wo_watcher_user_mapping_id = table.Column<Guid>(nullable: false),
                    ref_id = table.Column<Guid>(nullable: false),
                    ref_type = table.Column<int>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    user_role_type = table.Column<int>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderWatcherUserMapping", x => x.wo_watcher_user_mapping_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderWatcherUserMapping");

            migrationBuilder.DropColumn(
                name: "notification_user_role",
                table: "NotificationData");
        }
    }
}
