using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class ExecutivePMDueReportConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEmailNotificationConfigurationSettings",
                columns: table => new
                {
                    email_config_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(nullable: false),
                    executive_pm_due_not_resolved_email_notification = table.Column<bool>(nullable: false),
                    disabled_till_date = table.Column<DateTime>(nullable: true),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEmailNotificationConfigurationSettings", x => x.email_config_id);
                    table.ForeignKey(
                        name: "FK_UserEmailNotificationConfigurationSettings_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailNotificationConfigurationSettings_user_id",
                table: "UserEmailNotificationConfigurationSettings",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEmailNotificationConfigurationSettings");
        }
    }
}
