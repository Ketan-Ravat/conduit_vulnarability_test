using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_pm_form_json_inAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<double>(
            //    name: "total_of_burden",
            //    table: "TimeMaterials",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "asset_pm_form_json",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "total_of_burden",
            //    table: "TimeMaterials");

            migrationBuilder.DropColumn(
                name: "asset_pm_form_json",
                table: "Assets");
        }
    }
}
