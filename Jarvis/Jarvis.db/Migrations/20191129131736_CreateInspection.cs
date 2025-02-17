using System;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CreateInspection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetsValueJsonObject",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(nullable: true),
                    value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetsValueJsonObject", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Inspection",
                columns: table => new
                {
                    inspection_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    operator_id = table.Column<string>(nullable: true),
                    manager_id = table.Column<string>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    operator_notes = table.Column<string>(nullable: true),
                    attribute_valuesid = table.Column<Guid>(nullable: true),
                    company_id = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    meter_hours = table.Column<long>(nullable: false),
                    shift = table.Column<int>(nullable: false),
                    image_list = table.Column<ImagesListObject>(type: "jsonb", nullable: true),
                    manager_notes = table.Column<string>(nullable: true),
                    datetime_requested = table.Column<DateTime>(nullable: true),
                    asset_id1 = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspection", x => x.inspection_id);
                    table.ForeignKey(
                        name: "FK_Inspection_Assets_asset_id1",
                        column: x => x.asset_id1,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspection_AssetsValueJsonObject_attribute_valuesid",
                        column: x => x.attribute_valuesid,
                        principalTable: "AssetsValueJsonObject",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inspection_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_asset_id1",
                table: "Inspection",
                column: "asset_id1");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_attribute_valuesid",
                table: "Inspection",
                column: "attribute_valuesid");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_site_id",
                table: "Inspection",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inspection");

            migrationBuilder.DropTable(
                name: "AssetsValueJsonObject");
        }
    }
}
