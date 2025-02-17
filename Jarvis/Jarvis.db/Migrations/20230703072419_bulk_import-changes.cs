using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class bulk_importchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bulk_data_import_failed_logs",
                table: "WorkOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bulk_data_import_status",
                table: "WorkOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bulk_data_import_failed_logs",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "bulk_data_import_status",
                table: "WorkOrders");
        }
    }
}
