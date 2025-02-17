using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WOcategorytoTaskMappingadded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment");
          
            migrationBuilder.CreateTable(
                name: "WOcategorytoTaskMapping",
                columns: table => new
                {
                    WOcategorytoTaskMapping_id = table.Column<Guid>(nullable: false),
                    wo_inspectionsTemplateFormIOAssignment_id = table.Column<Guid>(nullable: false),
                    task_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    updated_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<Guid>(nullable: true),
                    updated_by = table.Column<Guid>(nullable: true),
                    is_archived = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WOcategorytoTaskMapping", x => x.WOcategorytoTaskMapping_id);
                    table.ForeignKey(
                        name: "FK_WOcategorytoTaskMapping_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WOcategorytoTaskMapping_WOInspectionsTemplateFormIOAssignme~",
                        column: x => x.wo_inspectionsTemplateFormIOAssignment_id,
                        principalTable: "WOInspectionsTemplateFormIOAssignment",
                        principalColumn: "wo_inspectionsTemplateFormIOAssignment_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "wo_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_task_id",
                table: "WOcategorytoTaskMapping",
                column: "task_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WOcategorytoTaskMapping_wo_inspectionsTemplateFormIOAssignm~",
                table: "WOcategorytoTaskMapping",
                column: "wo_inspectionsTemplateFormIOAssignment_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WOcategorytoTaskMapping");

            migrationBuilder.DropIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment");

            migrationBuilder.CreateIndex(
                name: "IX_WOInspectionsTemplateFormIOAssignment_wo_id",
                table: "WOInspectionsTemplateFormIOAssignment",
                column: "wo_id");
        }
    }
}
