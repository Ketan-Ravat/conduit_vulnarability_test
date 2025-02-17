using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class assetformio_site_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "site_id",
                table: "AssetFormIO",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetFormIO_site_id",
                table: "AssetFormIO",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFormIO_Sites_site_id",
                table: "AssetFormIO",
                column: "site_id",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFormIO_Sites_site_id",
                table: "AssetFormIO");

            migrationBuilder.DropIndex(
                name: "IX_AssetFormIO_site_id",
                table: "AssetFormIO");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "AssetFormIO");
        }
    }
}
