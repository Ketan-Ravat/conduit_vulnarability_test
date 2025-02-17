using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOcategorytoTaskMapping_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "wo_id",
                table: "WOcategorytoTaskMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_wo_id",
                table: "WOcategorytoTaskMapping",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOcategorytoTaskMapping_WorkOrders_wo_id",
                table: "WOcategorytoTaskMapping",
                column: "wo_id",
                principalTable: "WorkOrders",
                principalColumn: "wo_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOcategorytoTaskMapping_WorkOrders_wo_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOcategorytoTaskMapping_wo_id",
                table: "WOcategorytoTaskMapping");

            migrationBuilder.DropColumn(
                name: "wo_id",
                table: "WOcategorytoTaskMapping");
        }
    }
}
