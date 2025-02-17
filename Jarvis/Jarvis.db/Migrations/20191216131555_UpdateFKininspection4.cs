using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateFKininspection4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_User_operator_id",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_operator_id",
                table: "Inspection");

            migrationBuilder.AddColumn<Guid>(
                name: "Useruuid",
                table: "Inspection",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_Useruuid",
                table: "Inspection",
                column: "Useruuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_User_Useruuid",
                table: "Inspection",
                column: "Useruuid",
                principalTable: "User",
                principalColumn: "uuid",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_User_Useruuid",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_Useruuid",
                table: "Inspection");

            migrationBuilder.DropColumn(
                name: "Useruuid",
                table: "Inspection");

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_operator_id",
                table: "Inspection",
                column: "operator_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_User_operator_id",
                table: "Inspection",
                column: "operator_id",
                principalTable: "User",
                principalColumn: "uuid",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
