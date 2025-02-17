using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ir_scan_label_photos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "s3_image_folder_name",
                table: "IRWOImagesLabelMapping",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetIRWOImagesLabelMapping",
                columns: table => new
                {
                    assetirwoimageslabelmapping_id = table.Column<Guid>(nullable: false),
                    ir_image_label = table.Column<string>(nullable: true),
                    visual_image_label = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    s3_image_folder_name = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetIRWOImagesLabelMapping", x => x.assetirwoimageslabelmapping_id);
                    table.ForeignKey(
                        name: "FK_AssetIRWOImagesLabelMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetIRWOImagesLabelMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetIRWOImagesLabelMapping_asset_id",
                table: "AssetIRWOImagesLabelMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetIRWOImagesLabelMapping_site_id",
                table: "AssetIRWOImagesLabelMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetIRWOImagesLabelMapping");

            migrationBuilder.DropColumn(
                name: "s3_image_folder_name",
                table: "IRWOImagesLabelMapping");
        }
    }
}
