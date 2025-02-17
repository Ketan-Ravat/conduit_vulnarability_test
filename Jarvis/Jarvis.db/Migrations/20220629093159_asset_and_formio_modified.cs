using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_and_formio_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_inspected_formio_date",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "intial_form_filled_date",
                table: "AssetFormIO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_inspected_formio_date",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "intial_form_filled_date",
                table: "AssetFormIO");
        }
    }
}
