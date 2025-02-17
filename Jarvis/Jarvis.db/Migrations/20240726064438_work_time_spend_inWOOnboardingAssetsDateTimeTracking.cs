using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class work_time_spend_inWOOnboardingAssetsDateTimeTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "work_time_spend",
                table: "WOOnboardingAssetsDateTimeTracking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "work_time_spend",
                table: "WOOnboardingAssetsDateTimeTracking");
        }
    }
}
