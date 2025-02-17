using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class obasset_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "fed_by_ob_asset_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_fed_by_ob_asset",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "other_notes",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fed_by_ob_asset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_fed_by_ob_asset",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "other_notes",
                table: "WOOnboardingAssets");
        }
    }
}
