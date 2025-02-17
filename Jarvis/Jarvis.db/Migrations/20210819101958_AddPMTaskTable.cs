using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class AddPMTaskTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "plan_code",
                table: "PMPlans");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    task_id = table.Column<Guid>(nullable: false),
                    task_title = table.Column<string>(nullable: true),
                    task_code = table.Column<string>(nullable: true),
                    task_est_minutes = table.Column<int>(nullable: false),
                    task_est_hours = table.Column<int>(nullable: false),
                    task_est_display = table.Column<string>(nullable: true),
                    isArchive = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.task_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "plan_code",
                table: "PMPlans",
                type: "text",
                nullable: true);
        }
    }
}
