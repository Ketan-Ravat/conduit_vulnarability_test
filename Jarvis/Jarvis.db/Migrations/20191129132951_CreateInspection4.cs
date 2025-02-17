using System;
using Jarvis.db.Models;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class CreateInspection4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inspection_AssetsValueJsonObject_attribute_valuesid",
                table: "Inspection");

            migrationBuilder.DropTable(
                name: "AssetsValueJsonObject");

            migrationBuilder.DropIndex(
                name: "IX_Inspection_attribute_valuesid",
                table: "Inspection");

            migrationBuilder.DropColumn(
                name: "attribute_valuesid",
                table: "Inspection");

            migrationBuilder.AddColumn<AssetsValueJsonObject>(
                name: "attribute_values",
                table: "Inspection",
                type: "jsonb",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "attribute_values",
                table: "Inspection");

            migrationBuilder.AddColumn<Guid>(
                name: "attribute_valuesid",
                table: "Inspection",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetsValueJsonObject",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetsValueJsonObject", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inspection_attribute_valuesid",
                table: "Inspection",
                column: "attribute_valuesid");

            migrationBuilder.AddForeignKey(
                name: "FK_Inspection_AssetsValueJsonObject_attribute_valuesid",
                table: "Inspection",
                column: "attribute_valuesid",
                principalTable: "AssetsValueJsonObject",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
