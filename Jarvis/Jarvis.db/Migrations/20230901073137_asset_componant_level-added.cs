using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_componant_leveladded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "circuit",
                table: "WOlineSubLevelcomponantMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "image_name",
                table: "WOlineSubLevelcomponantMapping",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "componant_level_type_id",
                table: "Assets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssetSubLevelcomponantMapping",
                columns: table => new
                {
                    asset_sublevelcomponant_mapping_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    sublevelcomponant_asset_id = table.Column<Guid>(nullable: false),
                    circuit = table.Column<string>(nullable: true),
                    image_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetSubLevelcomponantMapping", x => x.asset_sublevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetSubLevelcomponantMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSubLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetTopLevelcomponantMapping",
                columns: table => new
                {
                    asset_toplevelcomponant_mapping_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    toplevelcomponant_asset_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTopLevelcomponantMapping", x => x.asset_toplevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetTopLevelcomponantMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetTopLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubLevelcomponantMapping_asset_id",
                table: "AssetSubLevelcomponantMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubLevelcomponantMapping_site_id",
                table: "AssetSubLevelcomponantMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTopLevelcomponantMapping_asset_id",
                table: "AssetTopLevelcomponantMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTopLevelcomponantMapping_site_id",
                table: "AssetTopLevelcomponantMapping",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSubLevelcomponantMapping");

            migrationBuilder.DropTable(
                name: "AssetTopLevelcomponantMapping");

            migrationBuilder.DropColumn(
                name: "circuit",
                table: "WOlineSubLevelcomponantMapping");

            migrationBuilder.DropColumn(
                name: "image_name",
                table: "WOlineSubLevelcomponantMapping");

            migrationBuilder.DropColumn(
                name: "componant_level_type_id",
                table: "Assets");
        }
    }
}
