using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Addnotificationtable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationData",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    target_type = table.Column<int>(nullable: false),
                    device_key = table.Column<string>(nullable: true),
                    ref_id = table.Column<string>(nullable: true),
                    heading = table.Column<string>(nullable: true),
                    message = table.Column<string>(nullable: true),
                    data = table.Column<string>(type: "jsonb", nullable: true),
                    notification_type = table.Column<int>(nullable: false),
                    createdDate = table.Column<DateTime>(nullable: false),
                    sendDate = table.Column<DateTime>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    OS = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationData", x => x.notification_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationData");
        }
    }
}
