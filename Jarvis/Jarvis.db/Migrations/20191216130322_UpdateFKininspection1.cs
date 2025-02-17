using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class UpdateFKininspection1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "operator_id",
            table: "Inspection");

            migrationBuilder.AddColumn<Guid>(
                name: "operator_id",
                table: "Inspection",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_User_operator_id",
                table: "Inspection");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_operator_id",
                table: "Inspection");

            migrationBuilder.AlterColumn<string>(
                name: "operator_id",
                table: "Inspection",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
