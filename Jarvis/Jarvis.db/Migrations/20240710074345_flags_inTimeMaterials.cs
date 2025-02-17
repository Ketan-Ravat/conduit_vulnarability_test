using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class flags_inTimeMaterials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_burden_enabled",
                table: "TimeMaterials",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_markup_enabled",
                table: "TimeMaterials",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddColumn<double>(
            //    name: "total_of_burden",
            //    table: "TimeMaterials",
            //    nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_burden_enabled",
                table: "TimeMaterials");

            migrationBuilder.DropColumn(
                name: "is_markup_enabled",
                table: "TimeMaterials");

            //migrationBuilder.DropColumn(
            //    name: "total_of_burden",
            //    table: "TimeMaterials");
        }
    }
}
