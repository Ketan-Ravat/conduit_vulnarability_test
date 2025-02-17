using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class multiparent_asset_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetChildrenHierarchyMapping",
                columns: table => new
                {
                    asset_children_hierrachy_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true),
                    children_asset_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetChildrenHierarchyMapping", x => x.asset_children_hierrachy_id);
                    table.ForeignKey(
                        name: "FK_AssetChildrenHierarchyMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetParentHierarchyMapping",
                columns: table => new
                {
                    asset_parent_hierrachy_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: true),
                    parent_asset_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetParentHierarchyMapping", x => x.asset_parent_hierrachy_id);
                    table.ForeignKey(
                        name: "FK_AssetParentHierarchyMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetParentHierarchyMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WOOBAssetFedByMapping",
                columns: table => new
                {
                    wo_ob_asset_fed_by_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: true),
                    site_id = table.Column<Guid>(nullable: true),
                    parent_asset_id = table.Column<Guid>(nullable: false),
                    is_parent_from_ob_wo = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOOBAssetFedByMapping", x => x.wo_ob_asset_fed_by_id);
                    table.ForeignKey(
                        name: "FK_WOOBAssetFedByMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOOBAssetFedByMapping_WOOnboardingAssets_woonboardingassets~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetChildrenHierarchyMapping_asset_id",
                table: "AssetChildrenHierarchyMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParentHierarchyMapping_asset_id",
                table: "AssetParentHierarchyMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetParentHierarchyMapping_site_id",
                table: "AssetParentHierarchyMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetFedByMapping_site_id",
                table: "WOOBAssetFedByMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOOBAssetFedByMapping_woonboardingassets_id",
                table: "WOOBAssetFedByMapping",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropTable(
                name: "AssetParentHierarchyMapping");

            migrationBuilder.DropTable(
                name: "WOOBAssetFedByMapping");
        }
    }
}
