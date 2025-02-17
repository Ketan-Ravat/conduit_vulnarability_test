using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddCompanyIdInPMCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isManagerNotes",
                table: "Sites",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "PMCategory",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PMCategory_company_id",
                table: "PMCategory",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_PMCategory_Company_company_id",
                table: "PMCategory",
                column: "company_id",
                principalTable: "Company",
                principalColumn: "company_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PMCategory_Company_company_id",
                table: "PMCategory");

            migrationBuilder.DropIndex(
                name: "IX_PMCategory_company_id",
                table: "PMCategory");

            migrationBuilder.DropColumn(
                name: "isManagerNotes",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "PMCategory");
        }
    }
}
