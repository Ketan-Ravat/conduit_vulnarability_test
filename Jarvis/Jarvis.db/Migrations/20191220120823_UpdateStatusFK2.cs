using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateStatusFK2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WorkOrder_inspection_id",
                table: "WorkOrder",
                column: "inspection_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder",
                column: "inspection_id",
                principalTable: "Inspection",
                principalColumn: "inspection_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkOrder_Inspection_inspection_id",
                table: "WorkOrder");

            migrationBuilder.DropIndex(
                name: "IX_WorkOrder_inspection_id",
                table: "WorkOrder");
        }
    }
}
