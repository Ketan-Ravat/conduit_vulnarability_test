using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class TimeMaterialsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TimeMaterials",
                columns: table => new
                {
                    time_material_id = table.Column<Guid>(nullable: false),
                    time_material_category_type = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    no_sub_flag = table.Column<bool>(nullable: false),
                    quantity = table.Column<double>(nullable: false),
                    quantity_unit_type = table.Column<int>(nullable: false),
                    rate = table.Column<double>(nullable: false),
                    amount = table.Column<double>(nullable: false),
                    markup = table.Column<double>(nullable: false),
                    total_of_markup = table.Column<double>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: true),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeMaterials", x => x.time_material_id);
                    table.ForeignKey(
                        name: "FK_TimeMaterials_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TimeMaterials_wo_id",
                table: "TimeMaterials",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimeMaterials");
        }
    }
}
