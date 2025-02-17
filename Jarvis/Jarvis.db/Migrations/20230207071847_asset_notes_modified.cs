using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_notes_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_form_id",
                table: "AssetNotes",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "woonboardingassets_id",
                table: "AssetNotes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetNotes_asset_form_id",
                table: "AssetNotes",
                column: "asset_form_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNotes_woonboardingassets_id",
                table: "AssetNotes",
                column: "woonboardingassets_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetNotes_AssetFormIO_asset_form_id",
                table: "AssetNotes",
                column: "asset_form_id",
                principalTable: "AssetFormIO",
                principalColumn: "asset_form_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetNotes_WOOnboardingAssets_woonboardingassets_id",
                table: "AssetNotes",
                column: "woonboardingassets_id",
                principalTable: "WOOnboardingAssets",
                principalColumn: "woonboardingassets_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetNotes_AssetFormIO_asset_form_id",
                table: "AssetNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetNotes_WOOnboardingAssets_woonboardingassets_id",
                table: "AssetNotes");

            migrationBuilder.DropIndex(
                name: "IX_AssetNotes_asset_form_id",
                table: "AssetNotes");

            migrationBuilder.DropIndex(
                name: "IX_AssetNotes_woonboardingassets_id",
                table: "AssetNotes");

            migrationBuilder.DropColumn(
                name: "asset_form_id",
                table: "AssetNotes");

            migrationBuilder.DropColumn(
                name: "woonboardingassets_id",
                table: "AssetNotes");
        }
    }
}
