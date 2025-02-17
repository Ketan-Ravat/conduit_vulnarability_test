using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class task_descriptionnotes_and_formiowork_procedureadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "work_procedure",
                table: "InspectionsTemplateFormIO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "work_procedure",
                table: "InspectionsTemplateFormIO");
        }
    }
}
