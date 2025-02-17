using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class code_compliance_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "code_compliance",
                table: "WOOnboardingAssets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "code_compliance",
                table: "Assets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "code_compliance",
                table: "WOOnboardingAssets");

            migrationBuilder.DropColumn(
                name: "code_compliance",
                table: "Assets");
        }
    }
}
