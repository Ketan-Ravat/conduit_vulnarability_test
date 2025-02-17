using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateStatusAssetFK1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_StatusMasters_StatusMasterstatus_id",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_StatusMasterstatus_id",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "StatusMasterstatus_id",
                table: "Assets");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_status",
                table: "Assets",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_StatusMasters_status",
                table: "Assets",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_StatusMasters_status",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_status",
                table: "Assets");

            migrationBuilder.AddColumn<int>(
                name: "StatusMasterstatus_id",
                table: "Assets",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_StatusMasterstatus_id",
                table: "Assets",
                column: "StatusMasterstatus_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_StatusMasters_StatusMasterstatus_id",
                table: "Assets",
                column: "StatusMasterstatus_id",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
