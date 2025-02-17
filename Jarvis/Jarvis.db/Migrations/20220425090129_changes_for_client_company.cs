using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class changes_for_client_company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ac_active_client_company",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ac_default_client_company",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "parent_company_id",
                table: "ClientCompany",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_active_client_company",
                table: "User",
                column: "ac_active_client_company");

            migrationBuilder.CreateIndex(
                name: "IX_User_ac_default_client_company",
                table: "User",
                column: "ac_default_client_company");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCompany_parent_company_id",
                table: "ClientCompany",
                column: "parent_company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientCompany_Company_parent_company_id",
                table: "ClientCompany",
                column: "parent_company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_ClientCompany_ac_active_client_company",
                table: "User",
                column: "ac_active_client_company",
                principalTable: "ClientCompany",
                principalColumn: "client_company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_ClientCompany_ac_default_client_company",
                table: "User",
                column: "ac_default_client_company",
                principalTable: "ClientCompany",
                principalColumn: "client_company_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientCompany_Company_parent_company_id",
                table: "ClientCompany");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ClientCompany_ac_active_client_company",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_ClientCompany_ac_default_client_company",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_active_client_company",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_ac_default_client_company",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_ClientCompany_parent_company_id",
                table: "ClientCompany");

            migrationBuilder.DropColumn(
                name: "ac_active_client_company",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ac_default_client_company",
                table: "User");

            migrationBuilder.DropColumn(
                name: "parent_company_id",
                table: "ClientCompany");
        }
    }
}
