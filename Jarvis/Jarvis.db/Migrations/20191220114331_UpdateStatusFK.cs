using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateStatusFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Inspection_status",
                table: "Inspection",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Company_status",
                table: "Company",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_StatusMasters_status",
                table: "Company",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_StatusMasters_status",
                table: "Inspection",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_StatusMasters_status",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_StatusMasters_status",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_status",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Company_status",
                table: "Company");
        }
    }
}
