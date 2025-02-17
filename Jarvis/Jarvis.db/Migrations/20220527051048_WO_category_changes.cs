using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WO_category_changes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "task_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_parent_task",
                table: "WOcategorytoTaskMapping",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_task_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "task_id");

            migrationBuilder.AddForeignKey(
                name: "FK_WOInspectionsTemplateFormIOAssignment_Tasks_task_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "task_id",
                principalTable: "Tasks",
                principalColumn: "task_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WOInspectionsTemplateFormIOAssignment_Tasks_task_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_task_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.DropColumn(
                name: "task_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.DropColumn(
                name: "is_parent_task",
                table: "WOcategorytoTaskMapping");
        }
    }
}
