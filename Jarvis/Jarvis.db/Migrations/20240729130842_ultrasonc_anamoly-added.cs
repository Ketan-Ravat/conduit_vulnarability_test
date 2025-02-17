using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class ultrasonc_anamolyadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location_of_ultrasonic_anamoly",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "size_of_ultrasonic_anamoly",
                table: "WOLineIssue",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type_of_ultrasonic_anamoly",
                table: "WOLineIssue",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location_of_ultrasonic_anamoly",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "size_of_ultrasonic_anamoly",
                table: "WOLineIssue");

            migrationBuilder.DropColumn(
                name: "type_of_ultrasonic_anamoly",
                table: "WOLineIssue");
        }
    }
}
