using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddEmailIsVerifiedInUserAddEmailNotStatusTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_email_verified",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EmailNotificationStatusUpdate",
                columns: table => new
                {
                    notification_id = table.Column<Guid>(nullable: false),
                    from = table.Column<string>(nullable: true),
                    to = table.Column<string>(nullable: true),
                    status = table.Column<string>(nullable: true),
                    subject = table.Column<string>(nullable: true),
                    submitted_on = table.Column<DateTime>(nullable: false),
                    retry_on = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailNotificationStatusUpdate", x => x.notification_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailNotificationStatusUpdate");

            migrationBuilder.DropColumn(
                name: "is_email_verified",
                table: "User");
        }
    }
}
