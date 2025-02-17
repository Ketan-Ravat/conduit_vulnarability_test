using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddedmainLocationidToTempLocationmodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "formioroom_id",
                table: "TempFormIORooms",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "formiofloor_id",
                table: "TempFormIOFloors",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "formiobuilding_id",
                table: "TempFormIOBuildings",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIORooms_formioroom_id",
                table: "TempFormIORooms",
                column: "formioroom_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOFloors_formiofloor_id",
                table: "TempFormIOFloors",
                column: "formiofloor_id");

            migrationBuilder.CreateIndex(
                name: "IX_TempFormIOBuildings_formiobuilding_id",
                table: "TempFormIOBuildings",
                column: "formiobuilding_id");

            migrationBuilder.AddForeignKey(
                name: "FK_TempFormIOBuildings_FormIOBuildings_formiobuilding_id",
                table: "TempFormIOBuildings",
                column: "formiobuilding_id",
                principalTable: "FormIOBuildings",
                principalColumn: "formiobuilding_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TempFormIOFloors_FormIOFloors_formiofloor_id",
                table: "TempFormIOFloors",
                column: "formiofloor_id",
                principalTable: "FormIOFloors",
                principalColumn: "formiofloor_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TempFormIORooms_FormIORooms_formioroom_id",
                table: "TempFormIORooms",
                column: "formioroom_id",
                principalTable: "FormIORooms",
                principalColumn: "formioroom_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TempFormIOBuildings_FormIOBuildings_formiobuilding_id",
                table: "TempFormIOBuildings");

            migrationBuilder.DropForeignKey(
                name: "FK_TempFormIOFloors_FormIOFloors_formiofloor_id",
                table: "TempFormIOFloors");

            migrationBuilder.DropForeignKey(
                name: "FK_TempFormIORooms_FormIORooms_formioroom_id",
                table: "TempFormIORooms");

            migrationBuilder.DropIndex(
                name: "IX_TempFormIORooms_formioroom_id",
                table: "TempFormIORooms");

            migrationBuilder.DropIndex(
                name: "IX_TempFormIOFloors_formiofloor_id",
                table: "TempFormIOFloors");

            migrationBuilder.DropIndex(
                name: "IX_TempFormIOBuildings_formiobuilding_id",
                table: "TempFormIOBuildings");

            migrationBuilder.DropColumn(
                name: "formioroom_id",
                table: "TempFormIORooms");

            migrationBuilder.DropColumn(
                name: "formiofloor_id",
                table: "TempFormIOFloors");

            migrationBuilder.DropColumn(
                name: "formiobuilding_id",
                table: "TempFormIOBuildings");
        }
    }
}
