using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class changekey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "created_datetime",
                table: "WorkOrderRecord");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "WorkOrderRecord",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "checkout_datetime",
                table: "WorkOrderRecord",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "WorkOrderRecord",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "inspection_id",
                table: "WorkOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "attribute_id",
                table: "WorkOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "WorkOrder",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder",
                column: "inspection_id",
                principalTable: "Inspection",
                principalColumn: "inspection_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder");

            migrationBuilder.DropColumn(
                name: "checkout_datetime",
                table: "WorkOrderRecord");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "WorkOrderRecord");

            migrationBuilder.AlterColumn<Guid>(
                name: "created_by",
                table: "WorkOrderRecord",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_datetime",
                table: "WorkOrderRecord",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "inspection_id",
                table: "WorkOrder",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "attribute_id",
                table: "WorkOrder",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "WorkOrder",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Assets_asset_id",
                table: "WorkOrder",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder",
                column: "inspection_id",
                principalTable: "Inspection",
                principalColumn: "inspection_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
