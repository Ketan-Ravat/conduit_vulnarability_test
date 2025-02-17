using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class asset_building_mapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormIOBuildings",
                columns: table => new
                {
                    formiobuilding_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    formio_building_name = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIOBuildings", x => x.formiobuilding_id);
                });

            migrationBuilder.CreateTable(
                name: "FormIOFloors",
                columns: table => new
                {
                    formiofloor_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    formio_floor_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    formiobuilding_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIOFloors", x => x.formiofloor_id);
                    table.ForeignKey(
                        name: "FK_FormIOFloors_FormIOBuildings_formiobuilding_id",
                        column: x => x.formiobuilding_id,
                        principalTable: "FormIOBuildings",
                        principalColumn: "formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormIORooms",
                columns: table => new
                {
                    formioroom_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    formio_room_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    formiofloor_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIORooms", x => x.formioroom_id);
                    table.ForeignKey(
                        name: "FK_FormIORooms_FormIOFloors_formiofloor_id",
                        column: x => x.formiofloor_id,
                        principalTable: "FormIOFloors",
                        principalColumn: "formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormIOSections",
                columns: table => new
                {
                    formiosection_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    formio_section_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    formioroom_id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormIOSections", x => x.formiosection_id);
                    table.ForeignKey(
                        name: "FK_FormIOSections_FormIORooms_formioroom_id",
                        column: x => x.formioroom_id,
                        principalTable: "FormIORooms",
                        principalColumn: "formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetFormIOBuildingMappings",
                columns: table => new
                {
                    assetformiobuildings_id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_at = table.Column<DateTime>(nullable: true),
                    formiobuilding_id = table.Column<int>(nullable: true),
                    formiofloor_id = table.Column<int>(nullable: true),
                    formioroom_id = table.Column<int>(nullable: true),
                    formiosection_id = table.Column<int>(nullable: true),
                    asset_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetFormIOBuildingMappings", x => x.assetformiobuildings_id);
                    table.ForeignKey(
                        name: "FK_AssetFormIOBuildingMappings_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetFormIOBuildingMappings_FormIOBuildings_formiobuilding_~",
                        column: x => x.formiobuilding_id,
                        principalTable: "FormIOBuildings",
                        principalColumn: "formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetFormIOBuildingMappings_FormIOFloors_formiofloor_id",
                        column: x => x.formiofloor_id,
                        principalTable: "FormIOFloors",
                        principalColumn: "formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetFormIOBuildingMappings_FormIORooms_formioroom_id",
                        column: x => x.formioroom_id,
                        principalTable: "FormIORooms",
                        principalColumn: "formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetFormIOBuildingMappings_FormIOSections_formiosection_id",
                        column: x => x.formiosection_id,
                        principalTable: "FormIOSections",
                        principalColumn: "formiosection_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIOBuildingMappings_asset_id",
                table: "AssetFormIOBuildingMappings",
                column: "asset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIOBuildingMappings_formiobuilding_id",
                table: "AssetFormIOBuildingMappings",
                column: "formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIOBuildingMappings_formiofloor_id",
                table: "AssetFormIOBuildingMappings",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIOBuildingMappings_formioroom_id",
                table: "AssetFormIOBuildingMappings",
                column: "formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIOBuildingMappings_formiosection_id",
                table: "AssetFormIOBuildingMappings",
                column: "formiosection_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIOFloors_formiobuilding_id",
                table: "FormIOFloors",
                column: "formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIORooms_formiofloor_id",
                table: "FormIORooms",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_FormIOSections_formioroom_id",
                table: "FormIOSections",
                column: "formioroom_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetFormIOBuildingMappings");

            migrationBuilder.DropTable(
                name: "FormIOSections");

            migrationBuilder.DropTable(
                name: "FormIORooms");

            migrationBuilder.DropTable(
                name: "FormIOFloors");

            migrationBuilder.DropTable(
                name: "FormIOBuildings");
        }
    }
}
