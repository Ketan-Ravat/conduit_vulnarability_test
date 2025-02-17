using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetExcelFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_StatusMasters_condition_index",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_condition_index",
                table: "Assets");

            migrationBuilder.AlterColumn<double>(
                name: "condition_index",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "asset_barcode_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "condition_state",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "criticality_index",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_condition_state",
                table: "Assets",
                column: "condition_state");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_StatusMasters_condition_state",
                table: "Assets",
                column: "condition_state",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_StatusMasters_condition_state",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_condition_state",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "asset_barcode_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "condition_state",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "criticality_index",
                table: "Assets");

            migrationBuilder.AlterColumn<int>(
                name: "condition_index",
                table: "Assets",
                type: "integer",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_condition_index",
                table: "Assets",
                column: "condition_index");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_StatusMasters_condition_index",
                table: "Assets",
                column: "condition_index",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
