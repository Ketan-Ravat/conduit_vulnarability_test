using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class adddafaultrolesites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_role_id",
                table: "User");

            migrationBuilder.AlterColumn<Guid>(
                name: "role_id",
                table: "User",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ac_default_role_app",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_default_role_web",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_default_site",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ti_default_role",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ti_default_site",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_default_role_app",
                table: "User",
                column: "ac_default_role_app");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_default_role_web",
                table: "User",
                column: "ac_default_role_web");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_default_site",
                table: "User",
                column: "ac_default_site");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_ac_default_role_app",
                table: "User",
                column: "ac_default_role_app",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_ac_default_role_web",
                table: "User",
                column: "ac_default_role_web",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Sites_ac_default_site",
                table: "User",
                column: "ac_default_site",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_role_id",
                table: "User",
                column: "role_id",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_ac_default_role_app",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_ac_default_role_web",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Sites_ac_default_site",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_role_id",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_default_role_app",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_default_role_web",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_default_site",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_default_role_app",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_default_role_web",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_default_site",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ti_default_role",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ti_default_site",
                table: "User");

            migrationBuilder.AlterColumn<Guid>(
                name: "role_id",
                table: "User",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_role_id",
                table: "User",
                column: "role_id",
                principalTable: "Role",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
