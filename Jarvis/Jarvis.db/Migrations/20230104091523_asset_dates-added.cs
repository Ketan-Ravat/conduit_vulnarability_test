using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_datesadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "electrical_insepction_last_performed",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "infrared_insepction_last_performed",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "mechanical_insepction_last_performed",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "visual_insepction_last_performed",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "electrical_insepction_last_performed",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "infrared_insepction_last_performed",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "mechanical_insepction_last_performed",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "visual_insepction_last_performed",
                table: "Assets");
        }
    }
}
