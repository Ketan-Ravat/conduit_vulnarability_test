using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class addactivesiteroleuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ac_active_role_app",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_active_role_web",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_active_site",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_active_role_app",
                table: "User",
                column: "ac_active_role_app");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_active_role_web",
                table: "User",
                column: "ac_active_role_web");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_active_site",
                table: "User",
                column: "ac_active_site");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_ac_active_role_app",
                table: "User",
                column: "ac_active_role_app",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_ac_active_role_web",
                table: "User",
                column: "ac_active_role_web",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Sites_ac_active_site",
                table: "User",
                column: "ac_active_site",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_ac_active_role_app",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_ac_active_role_web",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Sites_ac_active_site",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_active_role_app",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_active_role_web",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_active_site",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_active_role_app",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_active_role_web",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_active_site",
                table: "User");
        }
    }
}
