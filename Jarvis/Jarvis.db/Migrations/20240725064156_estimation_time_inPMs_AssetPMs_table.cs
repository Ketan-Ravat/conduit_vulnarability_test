using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class estimation_time_inPMs_AssetPMs_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estimation_time",
                table: "PMs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "estimation_time",
                table: "AssetPMs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estimation_time",
                table: "PMs");

            migrationBuilder.DropColumn(
                name: "estimation_time",
                table: "AssetPMs");
        }
    }
}
