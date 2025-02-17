using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CreateInspection2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Assets_asset_id",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_asset_id",
                table: "Inspection");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "Inspection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "Inspection",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_asset_id",
                table: "Inspection",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Assets_asset_id",
                table: "Inspection",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
