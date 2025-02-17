using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddServiceDealerInWorkOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "service_dealer_id",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrders_service_dealer_id",
                table: "WorkOrders",
                column: "service_dealer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_ServiceDealers_service_dealer_id",
                table: "WorkOrders",
                column: "service_dealer_id",
                principalTable: "ServiceDealers",
                principalColumn: "service_dealer_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_ServiceDealers_service_dealer_id",
                table: "WorkOrders");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrders_service_dealer_id",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "service_dealer_id",
                table: "WorkOrders");
        }
    }
}
