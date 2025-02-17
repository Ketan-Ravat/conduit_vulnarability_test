using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Equipmentcompany_idchanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "Equipment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_company_id",
                table: "Equipment",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Company_company_id",
                table: "Equipment",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Company_company_id",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_company_id",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "Equipment");
        }
    }
}
