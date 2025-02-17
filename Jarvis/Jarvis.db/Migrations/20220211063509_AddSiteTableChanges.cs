using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddSiteTableChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isAutoApprove",
                table: "Sites",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "showHideApprove",
                table: "Sites",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isAutoApprove",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "showHideApprove",
                table: "Sites");
        }
    }
}
