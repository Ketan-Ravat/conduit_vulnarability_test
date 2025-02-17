using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class pm_master_formsadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "asset_pm_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pm_form_output_data",
                table: "AssetPMs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PMItemMasterForms",
                columns: table => new
                {
                    pmitemmasterform_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: true),
                    form_json = table.Column<string>(nullable: true),
                    asset_class_code = table.Column<string>(nullable: true),
                    asset_class_name = table.Column<string>(nullable: true),
                    plan_name = table.Column<string>(nullable: true),
                    pm_title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PMItemMasterForms", x => x.pmitemmasterform_id);
                    table.ForeignKey(
                        name: "FK_PMItemMasterForms_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssets_asset_pm_id",
                table: "WOOnboardingAssets",
                column: "asset_pm_id");

            migrationBuilder.CreateIndex(
                name: "IX_PMItemMasterForms_company_id",
                table: "PMItemMasterForms",
                column: "company_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssets_AssetPMs_asset_pm_id",
                table: "WOOnboardingAssets",
                column: "asset_pm_id",
                principalTable: "AssetPMs",
                principalColumn: "asset_pm_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssets_AssetPMs_asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropTable(
                name: "PMItemMasterForms");

            migrationBuilder.DropIndex(
                name: "IX_WOOnboardingAssets_asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "asset_pm_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "pm_form_output_data",
                table: "AssetPMs");
        }
    }
}
