using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class clientcompanyadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_company_id",
                table: "Sites",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientCompany",
                columns: table => new
                {
                    client_company_id = table.Column<Guid>(nullable: false),
                    client_company_name = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    created_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCompany", x => x.client_company_id);
                    table.ForeignKey(
                        name: "FK_ClientCompany_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sites_client_company_id",
                table: "Sites",
                column: "client_company_id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCompany_status",
                table: "ClientCompany",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_ClientCompany_client_company_id",
                table: "Sites",
                column: "client_company_id",
                principalTable: "ClientCompany",
                principalColumn: "client_company_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sites_ClientCompany_client_company_id",
                table: "Sites");

            migrationBuilder.DropTable(
                name: "ClientCompany");

            migrationBuilder.DropIndex(
                name: "IX_Sites_client_company_id",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "client_company_id",
                table: "Sites");
        }
    }
}
