using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_notes_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetNotes",
                columns: table => new
                {
                    asset_notes_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    asset_note = table.Column<string>(nullable: true),
                    asset_note_added_by_userid = table.Column<Guid>(nullable: false),
                    asset_note_added_by_user = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<Guid>(nullable: true),
                    updated_by = table.Column<Guid>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetNotes", x => x.asset_notes_id);
                    table.ForeignKey(
                        name: "FK_AssetNotes_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetNotes_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetNotes_asset_id",
                table: "AssetNotes",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetNotes_site_id",
                table: "AssetNotes",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetNotes");
        }
    }
}
