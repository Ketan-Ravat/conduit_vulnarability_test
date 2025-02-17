using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class wotask_asset_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "assigned_asset",
                table: "WOcategorytoTaskMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping",
                column: "assigned_asset",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WOcategorytoTaskMapping_Assets_assigned_asset",
                table: "WOcategorytoTaskMapping",
                column: "assigned_asset",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOcategorytoTaskMapping_Assets_assigned_asset",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_assigned_asset",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "assigned_asset",
                table: "WOcategorytoTaskMapping");
        }
    }
}
