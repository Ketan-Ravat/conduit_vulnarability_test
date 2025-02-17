using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ChnageAssetBarcodeIdType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "asset_barcode_id",
                table: "Assets",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "asset_barcode_id",
                table: "Assets",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
