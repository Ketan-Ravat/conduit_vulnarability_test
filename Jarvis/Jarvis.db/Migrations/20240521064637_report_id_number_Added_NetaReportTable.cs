using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class report_id_number_Added_NetaReportTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "report_id_number",
                table: "NetaInspectionBulkReportTracking",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "report_id_number",
                table: "NetaInspectionBulkReportTracking");
        }
    }
}
