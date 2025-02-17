using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class assetformio_assetid_nullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_Assets_asset_id",
                table: "AssetFormIO");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "AssetFormIO",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_Assets_asset_id",
                table: "AssetFormIO",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_Assets_asset_id",
                table: "AssetFormIO");

            migrationBuilder.AlterColumn<Guid>(
                name: "asset_id",
                table: "AssetFormIO",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_Assets_asset_id",
                table: "AssetFormIO",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
