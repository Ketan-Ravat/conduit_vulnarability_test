using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class dynamic_nameplate_fields_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "dynamic_nameplate_fields",
                table: "InspectionsTemplateFormIO",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dynamic_nameplate_fields",
                table: "InspectionsTemplateFormIO");
        }
    }
}
