using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOOnboardingAsset_cols_added_inspected_at : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "completed_at",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "inspected_at",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completed_at",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "inspected_at",
                table: "WOOnboardingAssets");
        }
    }
}
