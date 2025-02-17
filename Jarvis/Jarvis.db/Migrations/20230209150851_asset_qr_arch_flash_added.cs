using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_qr_arch_flash_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QR_code",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "arc_flash_study_last_performed",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QR_code",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "arc_flash_study_last_performed",
                table: "Assets");
        }
    }
}
