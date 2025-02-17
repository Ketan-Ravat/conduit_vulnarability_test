using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CreateInspection3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "Inspection",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
