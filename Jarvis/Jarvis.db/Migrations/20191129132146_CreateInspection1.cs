using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CreateInspection1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Assets_asset_id1",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_asset_id1",
                table: "Inspection");

            migrationBuilder.DropColumn(
                name: "asset_id1",
                table: "Inspection");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "Inspection",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_Assets_asset_id",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_asset_id",
                table: "Inspection");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "Inspection",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "asset_id1",
                table: "Inspection",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_asset_id1",
                table: "Inspection",
                column: "asset_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_Assets_asset_id1",
                table: "Inspection",
                column: "asset_id1",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
