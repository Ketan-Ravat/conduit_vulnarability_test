using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class newkeyin_WOOBAssetImgMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOOnboardingAssets_woonboar~",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "woonboardingassets_id",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "woline_sublevelcomponent_mapping_id",
                table: "WOOnboardingAssetsImagesMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOOnboardingAssetsImagesMapping_woline_sublevelcomponent_ma~",
                table: "WOOnboardingAssetsImagesMapping",
                column: "woline_sublevelcomponent_mapping_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOlineSubLevelcomponentMapp~",
                table: "WOOnboardingAssetsImagesMapping",
                column: "woline_sublevelcomponent_mapping_id",
                principalTable: "WOlineSubLevelcomponentMapping",
                principalColumn: "woline_sublevelcomponent_mapping_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOOnboardingAssets_woonboar~",
                table: "WOOnboardingAssetsImagesMapping",
                column: "woonboardingassets_id",
                principalTable: "WOOnboardingAssets",
                principalColumn: "woonboardingassets_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOlineSubLevelcomponentMapp~",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOOnboardingAssets_woonboar~",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOOnboardingAssetsImagesMapping_woline_sublevelcomponent_ma~",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.DropColumn(
                name: "woline_sublevelcomponent_mapping_id",
                table: "WOOnboardingAssetsImagesMapping");

            migrationBuilder.AlterColumn<Guid>(
                name: "woonboardingassets_id",
                table: "WOOnboardingAssetsImagesMapping",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WOOnboardingAssetsImagesMapping_WOOnboardingAssets_woonboar~",
                table: "WOOnboardingAssetsImagesMapping",
                column: "woonboardingassets_id",
                principalTable: "WOOnboardingAssets",
                principalColumn: "woonboardingassets_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
