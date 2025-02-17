using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class EquipmentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    equipment_id = table.Column<Guid>(nullable: false),
                    equipment_number = table.Column<string>(nullable: true),
                    site_id = table.Column<Guid>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    manufacturer = table.Column<string>(nullable: true),
                    model_number = table.Column<string>(nullable: true),
                    serial_number = table.Column<string>(nullable: true),
                    calibration_interval = table.Column<int>(nullable: false),
                    calibration_date = table.Column<DateTime>(nullable: false),
                    calibration_status = table.Column<int>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    isarchive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.equipment_id);
                    table.ForeignKey(
                        name: "FK_Equipment_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_site_id",
                table: "Equipment",
                column: "site_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipment");
        }
    }
}
