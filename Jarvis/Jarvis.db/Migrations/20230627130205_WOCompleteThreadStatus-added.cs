using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOCompleteThreadStatusadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "complete_wo_thread_status",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_main_asset_created",
                table: "AssetFormIO",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "complete_wo_thread_status",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "is_main_asset_created",
                table: "AssetFormIO");
        }
    }
}
