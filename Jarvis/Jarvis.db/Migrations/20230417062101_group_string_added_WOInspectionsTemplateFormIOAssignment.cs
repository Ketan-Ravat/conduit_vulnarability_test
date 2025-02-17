using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class group_string_added_WOInspectionsTemplateFormIOAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "group_string",
                table: "WOInspectionsTemplateFormIOAssignment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "group_string",
                table: "WOInspectionsTemplateFormIOAssignment");
        }
    }
}
