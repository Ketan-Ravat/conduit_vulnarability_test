using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UserEmailNotificationConfigUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "UserEmailNotificationConfigurationSettings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "disable_till",
                table: "UserEmailNotificationConfigurationSettings",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "disable_till_by",
                table: "UserEmailNotificationConfigurationSettings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "UserEmailNotificationConfigurationSettings",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "setup_on",
                table: "UserEmailNotificationConfigurationSettings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserEmailNotificationConfigurationSettings_disable_till_by",
                table: "UserEmailNotificationConfigurationSettings",
                column: "disable_till_by");

            migrationBuilder.AddForeignKey(
                name: "FK_UserEmailNotificationConfigurationSettings_StatusMasters_di~",
                table: "UserEmailNotificationConfigurationSettings",
                column: "disable_till_by",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserEmailNotificationConfigurationSettings_StatusMasters_di~",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropIndex(
                name: "IX_UserEmailNotificationConfigurationSettings_disable_till_by",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "disable_till",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "disable_till_by",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "UserEmailNotificationConfigurationSettings");

            migrationBuilder.DropColumn(
                name: "setup_on",
                table: "UserEmailNotificationConfigurationSettings");
        }
    }
}
