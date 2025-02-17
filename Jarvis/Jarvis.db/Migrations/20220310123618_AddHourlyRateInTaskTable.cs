using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddHourlyRateInTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hourly_rate",
                table: "WorkOrderTasks");

            migrationBuilder.AddColumn<double>(
                name: "hourly_rate",
                table: "Tasks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hourly_rate",
                table: "Tasks");

            migrationBuilder.AddColumn<double>(
                name: "hourly_rate",
                table: "WorkOrderTasks",
                type: "double precision",
                nullable: true);
        }
    }
}
