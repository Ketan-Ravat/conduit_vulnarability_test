using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class company_logoadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "company_logo",
                table: "Company",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "company_thumbnail_logo",
                table: "Company",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetProfileImages",
                columns: table => new
                {
                    asset_profile_images_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    asset_photo = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetProfileImages", x => x.asset_profile_images_id);
                    table.ForeignKey(
                        name: "FK_AssetProfileImages_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetProfileImages_asset_id",
                table: "AssetProfileImages",
                column: "asset_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetProfileImages");

            migrationBuilder.DropColumn(
                name: "company_logo",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "company_thumbnail_logo",
                table: "Company");
        }
    }
}
