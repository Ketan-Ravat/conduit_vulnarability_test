using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class irwoimagelabelmapping_id_addedin_WOlineIssueImagesMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "irwoimagelabelmapping_id",
                table: "WOlineIssueImagesMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOlineIssueImagesMapping_irwoimagelabelmapping_id",
                table: "WOlineIssueImagesMapping",
                column: "irwoimagelabelmapping_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOlineIssueImagesMapping_IRWOImagesLabelMapping_irwoimagela~",
                table: "WOlineIssueImagesMapping",
                column: "irwoimagelabelmapping_id",
                principalTable: "IRWOImagesLabelMapping",
                principalColumn: "irwoimagelabelmapping_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOlineIssueImagesMapping_IRWOImagesLabelMapping_irwoimagela~",
                table: "WOlineIssueImagesMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOlineIssueImagesMapping_irwoimagelabelmapping_id",
                table: "WOlineIssueImagesMapping");

            migrationBuilder.DropColumn(
                name: "irwoimagelabelmapping_id",
                table: "WOlineIssueImagesMapping");
        }
    }
}
