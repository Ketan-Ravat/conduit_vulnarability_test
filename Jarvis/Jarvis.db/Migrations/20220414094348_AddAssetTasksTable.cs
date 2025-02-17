using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddAssetTasksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "form_id",
                table: "Tasks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "client_internal_id",
                table: "Assets",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetTasks",
                columns: table => new
                {
                    asset_task_id = table.Column<Guid>(nullable: false),
                    task_id = table.Column<Guid>(nullable: false),
                    asset_id = table.Column<Guid>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    is_archive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetTasks", x => x.asset_task_id);
                    table.ForeignKey(
                        name: "FK_AssetTasks_Assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "Assets",
                        principalColumn: "asset_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetTasks_StatusMasters_status",
                        column: x => x.status,
                        principalTable: "StatusMasters",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetTasks_Tasks_task_id",
                        column: x => x.task_id,
                        principalTable: "Tasks",
                        principalColumn: "task_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_form_id",
                table: "Tasks",
                column: "form_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTasks_asset_id",
                table: "AssetTasks",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTasks_status",
                table: "AssetTasks",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_AssetTasks_task_id",
                table: "AssetTasks",
                column: "task_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_InspectionsTemplateFormIO_form_id",
                table: "Tasks",
                column: "form_id",
                principalTable: "InspectionsTemplateFormIO",
                principalColumn: "form_id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_InspectionsTemplateFormIO_form_id",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "AssetTasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_form_id",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "form_id",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "client_internal_id",
                table: "Assets");
        }
    }
}
