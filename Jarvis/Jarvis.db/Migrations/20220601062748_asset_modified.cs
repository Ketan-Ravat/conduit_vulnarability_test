using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_newly_asset_created",
                table: "WOcategorytoTaskMapping",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "newly_created_asset_id",
                table: "WOcategorytoTaskMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Catalog_Number",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Identification",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Install_Date",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping",
                column: "newly_created_asset_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOcategorytoTaskMapping_Assets_newly_created_asset_id",
                table: "WOcategorytoTaskMapping",
                column: "newly_created_asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOcategorytoTaskMapping_Assets_newly_created_asset_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_newly_created_asset_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "is_newly_asset_created",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "newly_created_asset_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "Catalog_Number",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Identification",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Install_Date",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Assets");
        }
    }
}
