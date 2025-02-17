using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_attachmentadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetAttachmentMapping",
                columns: table => new
                {
                    assetatachmentmapping_id = table.Column<Guid>(nullable: false),
                    file_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetAttachmentMapping", x => x.assetatachmentmapping_id);
                    table.ForeignKey(
                        name: "FK_AssetAttachmentMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetAttachmentMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetAttachmentMapping_asset_id",
                table: "AssetAttachmentMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetAttachmentMapping_site_id",
                table: "AssetAttachmentMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetAttachmentMapping");
        }
    }
}
