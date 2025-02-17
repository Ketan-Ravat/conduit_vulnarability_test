using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ResponsibleParty_quote_status_WO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "quote_status",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "responsible_party_id",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResponsibleParty",
                columns: table => new
                {
                    responsible_party_id = table.Column<Guid>(nullable: false),
                    responsible_party_name = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibleParty", x => x.responsible_party_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_quote_status",
                table: "WorkOrders",
                column: "quote_status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_responsible_party_id",
                table: "WorkOrders",
                column: "responsible_party_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_StatusMasters_quote_status",
                table: "WorkOrders",
                column: "quote_status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ResponsibleParty_responsible_party_id",
                table: "WorkOrders",
                column: "responsible_party_id",
                principalTable: "ResponsibleParty",
                principalColumn: "responsible_party_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_StatusMasters_quote_status",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ResponsibleParty_responsible_party_id",
                table: "WorkOrders");

            migrationBuilder.DropTable(
                name: "ResponsibleParty");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_quote_status",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_responsible_party_id",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "quote_status",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "responsible_party_id",
                table: "WorkOrders");
        }
    }
}
