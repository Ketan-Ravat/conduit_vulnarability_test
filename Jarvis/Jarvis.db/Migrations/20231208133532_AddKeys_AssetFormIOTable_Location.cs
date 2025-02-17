using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddKeys_AssetFormIOTable_Location : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "building",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "floor",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "room",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "section",
                table: "AssetFormIO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "building",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "floor",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "room",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "section",
                table: "AssetFormIO");
        }
    }
}
