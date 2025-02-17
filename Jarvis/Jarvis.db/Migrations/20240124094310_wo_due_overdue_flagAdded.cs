using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class wo_due_overdue_flagAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "wo_due_overdue_flag",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "wo_due_time_duration",
                table: "WorkOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wo_due_overdue_flag",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "wo_due_time_duration",
                table: "WorkOrders");
        }
    }
}
