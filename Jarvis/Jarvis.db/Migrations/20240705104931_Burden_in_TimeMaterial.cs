using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class Burden_in_TimeMaterial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "burden",
                table: "TimeMaterials",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "burden_type",
                table: "TimeMaterials",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "total_of_burden",
                table: "TimeMaterials",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "burden",
                table: "TimeMaterials");

            migrationBuilder.DropColumn(
                name: "burden_type",
                table: "TimeMaterials");

            migrationBuilder.DropColumn(
                name: "total_of_burden",
                table: "TimeMaterials");
        }
    }
}
