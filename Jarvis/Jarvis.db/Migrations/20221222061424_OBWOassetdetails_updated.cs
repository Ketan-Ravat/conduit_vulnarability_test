using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class OBWOassetdetails_updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "WOOnboardingAssets",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modified_by",
                table: "WOOnboardingAssets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "modified_by",
                table: "WOOnboardingAssets");
        }
    }
}
