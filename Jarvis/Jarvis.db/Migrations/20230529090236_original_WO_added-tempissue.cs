using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class original_WO_addedtempissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "original_asset_form_id",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "original_wo_id",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "original_woonboardingassets_id",
                table: "WOLineIssue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "original_asset_form_id",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "original_wo_id",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "original_woonboardingassets_id",
                table: "WOLineIssue");
        }
    }
}
