using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class changetypefixedby : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "fixed_by",
                table: "WorkOrderRecord",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "fixed_by",
                table: "WorkOrderRecord",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
