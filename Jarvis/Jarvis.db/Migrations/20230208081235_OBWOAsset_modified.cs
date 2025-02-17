using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class OBWOAsset_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comments",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "inspection_further_details",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "inspection_type",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "mwo_date",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "mwo_inspection_type_status",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "problem_description",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recommended_action",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recommended_action_schedule",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "repair_resolution",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "replacement_resolution",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "solution_description",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "technician_user_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "inspection_type",
                table: "WOcategorytoTaskMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comments",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "inspection_further_details",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "mwo_date",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "problem_description",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recommended_action",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recommended_action_schedule",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "repair_resolution",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "replacement_resolution",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "solution_description",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_name",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_asset_id",
                table: "WOOnboardingAssets",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssets_Assets_asset_id",
                table: "WOOnboardingAssets",
                column: "asset_id",
                principalTable: "Assets",
                principalColumn: "asset_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssets_Assets_asset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropIndex(
                name: "IX_WOOnboardingAssets_asset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "comments",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "inspection_further_details",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "inspection_type",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "mwo_date",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "mwo_inspection_type_status",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "problem_description",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "recommended_action",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "recommended_action_schedule",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "repair_resolution",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "replacement_resolution",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "solution_description",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "technician_user_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "inspection_type",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "comments",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "inspection_further_details",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "mwo_date",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "problem_description",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "recommended_action",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "recommended_action_schedule",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "repair_resolution",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "replacement_resolution",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "solution_description",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "status_name",
                table: "Assets");
        }
    }
}
