using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class workorder_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Assets_asset_id",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_StatusMasters_priority",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "priority",
                table: "WorkOrders",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "WorkOrders",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "WorkOrders",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Assets_asset_id",
                table: "WorkOrders",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_StatusMasters_priority",
                table: "WorkOrders",
                column: "priority",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_Assets_asset_id",
                table: "WorkOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrders_StatusMasters_priority",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "WorkOrders");

            migrationBuilder.AlterColumn<int>(
                name: "priority",
                table: "WorkOrders",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "WorkOrders",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_Assets_asset_id",
                table: "WorkOrders",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrders_StatusMasters_priority",
                table: "WorkOrders",
                column: "priority",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
