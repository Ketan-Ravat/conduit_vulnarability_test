using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateStatusFK1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserSites_status",
                table: "UserSites",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_User_status",
                table: "User",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_status",
                table: "Sites",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_InspectionForms_status",
                table: "InspectionForms",
                column: "status");

            migrationBuilder.AddForeignKey(
                name: "FK_InspectionForms_StatusMasters_status",
                table: "InspectionForms",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sites_StatusMasters_status",
                table: "Sites",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_StatusMasters_status",
                table: "User",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSites_StatusMasters_status",
                table: "UserSites",
                column: "status",
                principalTable: "StatusMasters",
                principalColumn: "status_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InspectionForms_StatusMasters_status",
                table: "InspectionForms");

            migrationBuilder.DropForeignKey(
                name: "FK_Sites_StatusMasters_status",
                table: "Sites");

            migrationBuilder.DropForeignKey(
                name: "FK_User_StatusMasters_status",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSites_StatusMasters_status",
                table: "UserSites");

            migrationBuilder.DropIndex(
                name: "IX_UserSites_status",
                table: "UserSites");

            migrationBuilder.DropIndex(
                name: "IX_User_status",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_Sites_status",
                table: "Sites");

            migrationBuilder.DropIndex(
                name: "IX_InspectionForms_status",
                table: "InspectionForms");
        }
    }
}
