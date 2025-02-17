using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AssetFormIOmodifiedv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "WOcategorytoTaskMapping_id",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_asset_id",
                table: "Tasks",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_WOcategorytoTaskMapping_id",
                table: "AssetFormIO",
                column: "WOcategorytoTaskMapping_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_WOcategorytoTaskMapping_WOcategorytoTaskMapping~",
                table: "AssetFormIO",
                column: "WOcategorytoTaskMapping_id",
                principalTable: "WOcategorytoTaskMapping",
                principalColumn: "WOcategorytoTaskMapping_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Assets_asset_id",
                table: "Tasks",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_WOcategorytoTaskMapping_WOcategorytoTaskMapping~",
                table: "AssetFormIO");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Assets_asset_id",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_asset_id",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_WOcategorytoTaskMapping_id",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "WOcategorytoTaskMapping_id",
                table: "AssetFormIO");
        }
    }
}
