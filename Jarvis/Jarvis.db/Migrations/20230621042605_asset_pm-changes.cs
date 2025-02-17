using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_pmchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_form_id",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "wo_id",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_asset_form_id",
                table: "AssetPMs",
                column: "asset_form_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_wo_id",
                table: "AssetPMs",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPMs_AssetFormIO_asset_form_id",
                table: "AssetPMs",
                column: "asset_form_id",
                principalTable: "AssetFormIO",
                principalColumn: "asset_form_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPMs_WorkOrders_wo_id",
                table: "AssetPMs",
                column: "wo_id",
                principalTable: "WorkOrders",
                principalColumn: "wo_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetPMs_AssetFormIO_asset_form_id",
                table: "AssetPMs");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetPMs_WorkOrders_wo_id",
                table: "AssetPMs");

            migrationBuilder.DropIndex(
                name: "IX_AssetPMs_asset_form_id",
                table: "AssetPMs");

            migrationBuilder.DropIndex(
                name: "IX_AssetPMs_wo_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "asset_form_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "wo_id",
                table: "AssetPMs");
        }
    }
}
