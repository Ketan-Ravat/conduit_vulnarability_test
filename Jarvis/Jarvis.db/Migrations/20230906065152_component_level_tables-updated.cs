using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class component_level_tablesupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSubLevelcomponantMapping");

            migrationBuilder.DropTable(
                name: "AssetTopLevelcomponantMapping");

            migrationBuilder.DropTable(
                name: "WOlineSubLevelcomponantMapping");

            migrationBuilder.DropTable(
                name: "WOlineTopLevelcomponantMapping");

            migrationBuilder.DropColumn(
                name: "componant_level_type_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "componant_level_type_id",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "component_level_type_id",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "component_level_type_id",
                table: "Assets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssetSubLevelcomponentMapping",
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
                    table.PrimaryKey("PK_AssetSubLevelcomponentMapping", x => x.asset_sublevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetSubLevelcomponentMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetSubLevelcomponentMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetTopLevelcomponentMapping",
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
                    table.PrimaryKey("PK_AssetTopLevelcomponentMapping", x => x.asset_toplevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_AssetTopLevelcomponentMapping_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetTopLevelcomponentMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WOlineSubLevelcomponentMapping",
                columns: table => new
                {
                    woline_sublevelcomponent_mapping_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    sublevelcomponent_asset_id = table.Column<Guid>(nullable: false),
                    is_sublevelcomponent_from_ob_wo = table.Column<bool>(nullable: false),
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
                    table.PrimaryKey("PK_WOlineSubLevelcomponentMapping", x => x.woline_sublevelcomponent_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponentMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponentMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WOlineTopLevelcomponentMapping",
                columns: table => new
                {
                    woline_toplevelcomponent_mapping_id = table.Column<Guid>(nullable: false),
                    woonboardingassets_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: true),
                    toplevelcomponent_asset_id = table.Column<Guid>(nullable: false),
                    is_toplevelcomponent_from_ob_wo = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineTopLevelcomponentMapping", x => x.woline_toplevelcomponent_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponentMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponentMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubLevelcomponentMapping_asset_id",
                table: "AssetSubLevelcomponentMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetSubLevelcomponentMapping_site_id",
                table: "AssetSubLevelcomponentMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTopLevelcomponentMapping_asset_id",
                table: "AssetTopLevelcomponentMapping",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTopLevelcomponentMapping_site_id",
                table: "AssetTopLevelcomponentMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponentMapping_site_id",
                table: "WOlineSubLevelcomponentMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponentMapping_woonboardingassets_id",
                table: "WOlineSubLevelcomponentMapping",
                column: "woonboardingassets_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponentMapping_site_id",
                table: "WOlineTopLevelcomponentMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponentMapping_woonboardingassets_id",
                table: "WOlineTopLevelcomponentMapping",
                column: "woonboardingassets_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetSubLevelcomponentMapping");

            migrationBuilder.DropTable(
                name: "AssetTopLevelcomponentMapping");

            migrationBuilder.DropTable(
                name: "WOlineSubLevelcomponentMapping");

            migrationBuilder.DropTable(
                name: "WOlineTopLevelcomponentMapping");

            migrationBuilder.DropColumn(
                name: "component_level_type_id",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "component_level_type_id",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "componant_level_type_id",
                table: "WOOnboardingAssets",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "componant_level_type_id",
                table: "Assets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AssetSubLevelcomponantMapping",
                columns: table => new
                {
                    asset_sublevelcomponant_mapping_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    circuit = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    image_name = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sublevelcomponant_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
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
                    asset_toplevelcomponant_mapping_id = table.Column<Guid>(type: "uuid", nullable: false),
                    asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    toplevelcomponant_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "WOlineSubLevelcomponantMapping",
                columns: table => new
                {
                    woline_sublevelcomponant_mapping_id = table.Column<Guid>(type: "uuid", nullable: false),
                    circuit = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    image_name = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_sublevelcomponant_from_ob_wo = table.Column<bool>(type: "boolean", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sublevelcomponant_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    woonboardingassets_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineSubLevelcomponantMapping", x => x.woline_sublevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineSubLevelcomponantMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WOlineTopLevelcomponantMapping",
                columns: table => new
                {
                    woline_toplevelcomponant_mapping_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    is_toplevelcomponant_from_ob_wo = table.Column<bool>(type: "boolean", nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: true),
                    toplevelcomponant_asset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true),
                    woonboardingassets_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOlineTopLevelcomponantMapping", x => x.woline_toplevelcomponant_mapping_id);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponantMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WOlineTopLevelcomponantMapping_WOOnboardingAssets_woonboard~",
                        column: x => x.woonboardingassets_id,
                        principalTable: "WOOnboardingAssets",
                        principalColumn: "woonboardingassets_id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponantMapping_site_id",
                table: "WOlineSubLevelcomponantMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineSubLevelcomponantMapping_woonboardingassets_id",
                table: "WOlineSubLevelcomponantMapping",
                column: "woonboardingassets_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponantMapping_site_id",
                table: "WOlineTopLevelcomponantMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WOlineTopLevelcomponantMapping_woonboardingassets_id",
                table: "WOlineTopLevelcomponantMapping",
                column: "woonboardingassets_id");
        }
    }
}
