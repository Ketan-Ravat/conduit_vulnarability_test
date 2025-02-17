using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddServiceDealersInPM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "service_dealer_id",
                table: "PMs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "service_dealer_id",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceDealers",
                columns: table => new
                {
                    service_dealer_id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    is_archive = table.Column<bool>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDealers", x => x.service_dealer_id);
                    table.ForeignKey(
                        name: "FK_ServiceDealers_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PMs_service_dealer_id",
                table: "PMs",
                column: "service_dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_service_dealer_id",
                table: "AssetPMs",
                column: "service_dealer_id");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDealers_status",
                table: "ServiceDealers",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPMs_ServiceDealers_service_dealer_id",
                table: "AssetPMs",
                column: "service_dealer_id",
                principalTable: "ServiceDealers",
                principalColumn: "service_dealer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PMs_ServiceDealers_service_dealer_id",
                table: "PMs",
                column: "service_dealer_id",
                principalTable: "ServiceDealers",
                principalColumn: "service_dealer_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetPMs_ServiceDealers_service_dealer_id",
                table: "AssetPMs");

            migrationBuilder.DropForeignKey(
                name: "FK_PMs_ServiceDealers_service_dealer_id",
                table: "PMs");

            migrationBuilder.DropTable(
                name: "ServiceDealers");

            migrationBuilder.DropIndex(
                name: "IX_PMs_service_dealer_id",
                table: "PMs");

            migrationBuilder.DropIndex(
                name: "IX_AssetPMs_service_dealer_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "service_dealer_id",
                table: "PMs");

            migrationBuilder.DropColumn(
                name: "service_dealer_id",
                table: "AssetPMs");
        }
    }
}
