using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TempLocationTablesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TempFormIOBuildings",
                columns: table => new
                {
                    temp_formiobuilding_id = table.Column<Guid>(nullable: false),
                    temp_formio_building_name = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempFormIOBuildings", x => x.temp_formiobuilding_id);
                    table.ForeignKey(
                        name: "FK_TempFormIOBuildings_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempFormIOFloors",
                columns: table => new
                {
                    temp_formiofloor_id = table.Column<Guid>(nullable: false),
                    temp_formio_floor_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    temp_formiobuilding_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempFormIOFloors", x => x.temp_formiofloor_id);
                    table.ForeignKey(
                        name: "FK_TempFormIOFloors_TempFormIOBuildings_temp_formiobuilding_id",
                        column: x => x.temp_formiobuilding_id,
                        principalTable: "TempFormIOBuildings",
                        principalColumn: "temp_formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempFormIOFloors_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempFormIORooms",
                columns: table => new
                {
                    temp_formioroom_id = table.Column<Guid>(nullable: false),
                    temp_formio_room_name = table.Column<string>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    temp_formiofloor_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempFormIORooms", x => x.temp_formioroom_id);
                    table.ForeignKey(
                        name: "FK_TempFormIORooms_TempFormIOFloors_temp_formiofloor_id",
                        column: x => x.temp_formiofloor_id,
                        principalTable: "TempFormIOFloors",
                        principalColumn: "temp_formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempFormIORooms_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempFormIOSections",
                columns: table => new
                {
                    temp_formiosection_id = table.Column<Guid>(nullable: false),
                    temp_formio_section_name = table.Column<string>(nullable: true),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    temp_formioroom_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempFormIOSections", x => x.temp_formiosection_id);
                    table.ForeignKey(
                        name: "FK_TempFormIOSections_TempFormIORooms_temp_formioroom_id",
                        column: x => x.temp_formioroom_id,
                        principalTable: "TempFormIORooms",
                        principalColumn: "temp_formioroom_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempFormIOSections_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOBuildings_wo_id",
                table: "TempFormIOBuildings",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOFloors_temp_formiobuilding_id",
                table: "TempFormIOFloors",
                column: "temp_formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOFloors_wo_id",
                table: "TempFormIOFloors",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIORooms_temp_formiofloor_id",
                table: "TempFormIORooms",
                column: "temp_formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIORooms_wo_id",
                table: "TempFormIORooms",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOSections_temp_formioroom_id",
                table: "TempFormIOSections",
                column: "temp_formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOSections_wo_id",
                table: "TempFormIOSections",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TempFormIOSections");

            migrationBuilder.DropTable(
                name: "TempFormIORooms");

            migrationBuilder.DropTable(
                name: "TempFormIOFloors");

            migrationBuilder.DropTable(
                name: "TempFormIOBuildings");
        }
    }
}
