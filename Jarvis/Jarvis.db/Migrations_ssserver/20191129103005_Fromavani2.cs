using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Fromavani2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "status",
                table: "AssetTransactionHistory");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "AssetTransactionHistory",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "shift",
                table: "AssetTransactionHistory",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "meter_hours",
                table: "AssetTransactionHistory",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "usage",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.DropColumn(
               name: "status",
               table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "Assets",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<long>(
                name: "meter_hours",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "company_id",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "asset_requested_on",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "asset_requested_by",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<DateTime>(
                name: "asset_approved_on",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "asset_approved_by",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "children",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "parent",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetTransactionHistory_asset_id",
                table: "AssetTransactionHistory",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_inspectionform_id",
                table: "Assets",
                column: "inspectionform_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_site_id",
                table: "Assets",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets",
                column: "inspectionform_id",
                principalTable: "InspectionForms",
                principalColumn: "inspection_form_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Sites_site_id",
                table: "Assets",
                column: "site_id",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetTransactionHistory_Assets_asset_id",
                table: "AssetTransactionHistory",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_InspectionForms_inspectionform_id",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Sites_site_id",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetTransactionHistory_Assets_asset_id",
                table: "AssetTransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_AssetTransactionHistory_asset_id",
                table: "AssetTransactionHistory");

            migrationBuilder.DropIndex(
                name: "IX_Assets_inspectionform_id",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_site_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "children",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "parent",
                table: "Assets");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "AssetTransactionHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "shift",
                table: "AssetTransactionHistory",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "meter_hours",
                table: "AssetTransactionHistory",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "usage",
                table: "Assets",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "status",
                table: "Assets",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "Assets",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "meter_hours",
                table: "Assets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Assets",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "company_id",
                table: "Assets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "asset_requested_on",
                table: "Assets",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_requested_by",
                table: "Assets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "asset_approved_on",
                table: "Assets",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_approved_by",
                table: "Assets",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
