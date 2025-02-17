using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class sitecontact_cc_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_company_id",
                table: "SiteContact",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SiteContact_client_company_id",
                table: "SiteContact",
                column: "client_company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_SiteContact_ClientCompany_client_company_id",
                table: "SiteContact",
                column: "client_company_id",
                principalTable: "ClientCompany",
                principalColumn: "client_company_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteContact_ClientCompany_client_company_id",
                table: "SiteContact");

            migrationBuilder.DropIndex(
                name: "IX_SiteContact_client_company_id",
                table: "SiteContact");

            migrationBuilder.DropColumn(
                name: "client_company_id",
                table: "SiteContact");
        }
    }
}
