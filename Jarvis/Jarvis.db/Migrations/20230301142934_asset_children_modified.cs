using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class asset_children_modified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "AssetChildrenHierarchyMapping",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "created_by",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "AssetChildrenHierarchyMapping",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "site_id",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "updated_by",
                table: "AssetChildrenHierarchyMapping",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetChildrenHierarchyMapping_site_id",
                table: "AssetChildrenHierarchyMapping",
                column: "site_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetChildrenHierarchyMapping_Sites_site_id",
                table: "AssetChildrenHierarchyMapping",
                column: "site_id",
                principalTable: "Sites",
                principalColumn: "site_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetChildrenHierarchyMapping_Sites_site_id",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropIndex(
                name: "IX_AssetChildrenHierarchyMapping_site_id",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "created_by",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "site_id",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "AssetChildrenHierarchyMapping");

            migrationBuilder.DropColumn(
                name: "updated_by",
                table: "AssetChildrenHierarchyMapping");
        }
    }
}
