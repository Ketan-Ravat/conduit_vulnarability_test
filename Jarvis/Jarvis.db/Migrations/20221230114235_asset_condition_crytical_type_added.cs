using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_condition_crytical_type_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "commisiion_date",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "condition_index_type",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "criticality_index_type",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "condition_index_type",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "criticality_index_type",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commisiion_date",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "condition_index_type",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "criticality_index_type",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "condition_index_type",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "criticality_index_type",
                table: "Assets");
        }
    }
}
