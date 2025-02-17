using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Add_Default_AND_Active_Company_USER : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ac_active_company",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_default_company",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_active_company",
                table: "User",
                column: "ac_active_company");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_default_company",
                table: "User",
                column: "ac_default_company");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Company_ac_active_company",
                table: "User",
                column: "ac_active_company",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Company_ac_default_company",
                table: "User",
                column: "ac_default_company",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Company_ac_active_company",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Company_ac_default_company",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_active_company",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_default_company",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_active_company",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_default_company",
                table: "User");
        }
    }
}
