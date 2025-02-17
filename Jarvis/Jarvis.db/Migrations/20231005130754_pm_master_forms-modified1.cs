using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class pm_master_formsmodified1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssets_AssetPMs_asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropIndex(
                name: "IX_WOOnboardingAssets_asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.AddColumn<Guid>(
                name: "woonboardingassets_id",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetPMs_woonboardingassets_id",
                table: "AssetPMs",
                column: "woonboardingassets_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetPMs_WOOnboardingAssets_woonboardingassets_id",
                table: "AssetPMs",
                column: "woonboardingassets_id",
                principalTable: "WOOnboardingAssets",
                principalColumn: "woonboardingassets_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetPMs_WOOnboardingAssets_woonboardingassets_id",
                table: "AssetPMs");

            migrationBuilder.DropIndex(
                name: "IX_AssetPMs_woonboardingassets_id",
                table: "AssetPMs");

            migrationBuilder.DropColumn(
                name: "woonboardingassets_id",
                table: "AssetPMs");

            migrationBuilder.AddColumn<Guid>(
                name: "asset_pm_id",
                table: "WOOnboardingAssets",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_asset_pm_id",
                table: "WOOnboardingAssets",
                column: "asset_pm_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssets_AssetPMs_asset_pm_id",
                table: "WOOnboardingAssets",
                column: "asset_pm_id",
                principalTable: "AssetPMs",
                principalColumn: "asset_pm_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
