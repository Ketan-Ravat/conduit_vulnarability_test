using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class assetformiomodified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "accepted_by",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "wo_id",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_wo_id",
                table: "AssetFormIO",
                column: "wo_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_WorkOrders_wo_id",
                table: "AssetFormIO",
                column: "wo_id",
                principalTable: "WorkOrders",
                principalColumn: "wo_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_WorkOrders_wo_id",
                table: "AssetFormIO");

            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_wo_id",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "accepted_by",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "wo_id",
                table: "AssetFormIO");
        }
    }
}
