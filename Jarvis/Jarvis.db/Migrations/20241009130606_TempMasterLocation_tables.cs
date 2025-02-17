using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TempMasterLocation_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "temp_master_building_id",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "temp_master_floor_id",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "temp_master_room_id",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "temp_master_section",
                table: "TempAsset",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TempMasterBuilding",
                columns: table => new
                {
                    temp_master_building_id = table.Column<Guid>(nullable: false),
                    temp_master_building_name = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    formiobuilding_id = table.Column<int>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterBuilding", x => x.temp_master_building_id);
                    table.ForeignKey(
                        name: "FK_TempMasterBuilding_FormIOBuildings_formiobuilding_id",
                        column: x => x.formiobuilding_id,
                        principalTable: "FormIOBuildings",
                        principalColumn: "formiobuilding_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempMasterBuilding_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterBuildingWOMapping",
                columns: table => new
                {
                    temp_master_building_wo_mapping_id = table.Column<Guid>(nullable: false),
                    temp_master_building_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterBuildingWOMapping", x => x.temp_master_building_wo_mapping_id);
                    table.ForeignKey(
                        name: "FK_TempMasterBuildingWOMapping_TempMasterBuilding_temp_master_~",
                        column: x => x.temp_master_building_id,
                        principalTable: "TempMasterBuilding",
                        principalColumn: "temp_master_building_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempMasterBuildingWOMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterFloor",
                columns: table => new
                {
                    temp_master_floor_id = table.Column<Guid>(nullable: false),
                    temp_master_floor_name = table.Column<string>(nullable: true),
                    temp_master_building_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    formiofloor_id = table.Column<int>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterFloor", x => x.temp_master_floor_id);
                    table.ForeignKey(
                        name: "FK_TempMasterFloor_FormIOFloors_formiofloor_id",
                        column: x => x.formiofloor_id,
                        principalTable: "FormIOFloors",
                        principalColumn: "formiofloor_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempMasterFloor_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempMasterFloor_TempMasterBuilding_temp_master_building_id",
                        column: x => x.temp_master_building_id,
                        principalTable: "TempMasterBuilding",
                        principalColumn: "temp_master_building_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterFloorWOMapping",
                columns: table => new
                {
                    temp_master_floor_wo_mapping_id = table.Column<Guid>(nullable: false),
                    temp_master_floor_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    TempMasterBuildingWOMappingtemp_master_building_wo_mapping_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterFloorWOMapping", x => x.temp_master_floor_wo_mapping_id);
                    table.ForeignKey(
                        name: "FK_TempMasterFloorWOMapping_TempMasterBuildingWOMapping_TempMa~",
                        column: x => x.TempMasterBuildingWOMappingtemp_master_building_wo_mapping_id,
                        principalTable: "TempMasterBuildingWOMapping",
                        principalColumn: "temp_master_building_wo_mapping_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempMasterFloorWOMapping_TempMasterFloor_temp_master_floor_~",
                        column: x => x.temp_master_floor_id,
                        principalTable: "TempMasterFloor",
                        principalColumn: "temp_master_floor_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempMasterFloorWOMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterRoom",
                columns: table => new
                {
                    temp_master_room_id = table.Column<Guid>(nullable: false),
                    temp_master_room_name = table.Column<string>(nullable: true),
                    temp_master_floor_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    formioroom_id = table.Column<int>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterRoom", x => x.temp_master_room_id);
                    table.ForeignKey(
                        name: "FK_TempMasterRoom_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempMasterRoom_TempMasterFloor_temp_master_floor_id",
                        column: x => x.temp_master_floor_id,
                        principalTable: "TempMasterFloor",
                        principalColumn: "temp_master_floor_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TempMasterRoomWOMapping",
                columns: table => new
                {
                    temp_master_room_wo_mapping_id = table.Column<Guid>(nullable: false),
                    temp_master_room_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    TempMasterFloorWOMappingtemp_master_floor_wo_mapping_id = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempMasterRoomWOMapping", x => x.temp_master_room_wo_mapping_id);
                    table.ForeignKey(
                        name: "FK_TempMasterRoomWOMapping_TempMasterFloorWOMapping_TempMaster~",
                        column: x => x.TempMasterFloorWOMappingtemp_master_floor_wo_mapping_id,
                        principalTable: "TempMasterFloorWOMapping",
                        principalColumn: "temp_master_floor_wo_mapping_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TempMasterRoomWOMapping_TempMasterRoom_temp_master_room_id",
                        column: x => x.temp_master_room_id,
                        principalTable: "TempMasterRoom",
                        principalColumn: "temp_master_room_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TempMasterRoomWOMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_master_building_id",
                table: "TempAsset",
                column: "temp_master_building_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_master_floor_id",
                table: "TempAsset",
                column: "temp_master_floor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempAsset_temp_master_room_id",
                table: "TempAsset",
                column: "temp_master_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterBuilding_formiobuilding_id",
                table: "TempMasterBuilding",
                column: "formiobuilding_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterBuilding_site_id",
                table: "TempMasterBuilding",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterBuildingWOMapping_temp_master_building_id",
                table: "TempMasterBuildingWOMapping",
                column: "temp_master_building_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterBuildingWOMapping_wo_id",
                table: "TempMasterBuildingWOMapping",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloor_formiofloor_id",
                table: "TempMasterFloor",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloor_site_id",
                table: "TempMasterFloor",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloor_temp_master_building_id",
                table: "TempMasterFloor",
                column: "temp_master_building_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloorWOMapping_TempMasterBuildingWOMappingtemp_ma~",
                table: "TempMasterFloorWOMapping",
                column: "TempMasterBuildingWOMappingtemp_master_building_wo_mapping_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloorWOMapping_temp_master_floor_id",
                table: "TempMasterFloorWOMapping",
                column: "temp_master_floor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterFloorWOMapping_wo_id",
                table: "TempMasterFloorWOMapping",
                column: "wo_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoom_site_id",
                table: "TempMasterRoom",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoom_temp_master_floor_id",
                table: "TempMasterRoom",
                column: "temp_master_floor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoomWOMapping_TempMasterFloorWOMappingtemp_master~",
                table: "TempMasterRoomWOMapping",
                column: "TempMasterFloorWOMappingtemp_master_floor_wo_mapping_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoomWOMapping_temp_master_room_id",
                table: "TempMasterRoomWOMapping",
                column: "temp_master_room_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempMasterRoomWOMapping_wo_id",
                table: "TempMasterRoomWOMapping",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TempAsset_TempMasterBuilding_temp_master_building_id",
                table: "TempAsset",
                column: "temp_master_building_id",
                principalTable: "TempMasterBuilding",
                principalColumn: "temp_master_building_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TempAsset_TempMasterFloor_temp_master_floor_id",
                table: "TempAsset",
                column: "temp_master_floor_id",
                principalTable: "TempMasterFloor",
                principalColumn: "temp_master_floor_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TempAsset_TempMasterRoom_temp_master_room_id",
                table: "TempAsset",
                column: "temp_master_room_id",
                principalTable: "TempMasterRoom",
                principalColumn: "temp_master_room_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempAsset_TempMasterBuilding_temp_master_building_id",
                table: "TempAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_TempAsset_TempMasterFloor_temp_master_floor_id",
                table: "TempAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_TempAsset_TempMasterRoom_temp_master_room_id",
                table: "TempAsset");

            migrationBuilder.DropTable(
                name: "TempMasterRoomWOMapping");

            migrationBuilder.DropTable(
                name: "TempMasterFloorWOMapping");

            migrationBuilder.DropTable(
                name: "TempMasterRoom");

            migrationBuilder.DropTable(
                name: "TempMasterBuildingWOMapping");

            migrationBuilder.DropTable(
                name: "TempMasterFloor");

            migrationBuilder.DropTable(
                name: "TempMasterBuilding");

            migrationBuilder.DropIndex(
                name: "IX_TempAsset_temp_master_building_id",
                table: "TempAsset");

            migrationBuilder.DropIndex(
                name: "IX_TempAsset_temp_master_floor_id",
                table: "TempAsset");

            migrationBuilder.DropIndex(
                name: "IX_TempAsset_temp_master_room_id",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "temp_master_building_id",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "temp_master_floor_id",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "temp_master_room_id",
                table: "TempAsset");

            migrationBuilder.DropColumn(
                name: "temp_master_section",
                table: "TempAsset");
        }
    }
}
