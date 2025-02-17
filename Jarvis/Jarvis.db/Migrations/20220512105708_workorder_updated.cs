using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class workorder_updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "client_company_id",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "technician_user_id",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_client_company_id",
                table: "WorkOrders",
                column: "client_company_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_technician_user_id",
                table: "WorkOrders",
                column: "technician_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ClientCompany_client_company_id",
                table: "WorkOrders",
                column: "client_company_id",
                principalTable: "ClientCompany",
                principalColumn: "client_company_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_User_technician_user_id",
                table: "WorkOrders",
                column: "technician_user_id",
                principalTable: "User",
                principalColumn: "uuid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ClientCompany_client_company_id",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_User_technician_user_id",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_client_company_id",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_technician_user_id",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "client_company_id",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "technician_user_id",
                table: "WorkOrders");
        }
    }
}
