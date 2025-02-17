using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Jarvis.db.Migrations
{
    public partial class UpdateTaskCodeToDBGeneratedAutoIncrement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "task_code",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "company_id",
                table: "Tasks",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "task_number",
                table: "Tasks",
                nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company_id",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "task_number",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "task_code",
                table: "Tasks",
                type: "text",
                nullable: true);
        }
    }
}
