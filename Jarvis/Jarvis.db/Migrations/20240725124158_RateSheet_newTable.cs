using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class RateSheet_newTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RateSheet",
                columns: table => new
                {
                    ratesheet_id = table.Column<Guid>(nullable: false),
                    time_material_category_type = table.Column<int>(nullable: false),
                    description = table.Column<string>(nullable: true),
                    no_sub_flag = table.Column<bool>(nullable: false),
                    rate = table.Column<double>(nullable: false),
                    markup = table.Column<double>(nullable: false),
                    item_code = table.Column<string>(nullable: true),
                    burden = table.Column<double>(nullable: true),
                    burden_type = table.Column<int>(nullable: true),
                    is_burden_enabled = table.Column<bool>(nullable: false),
                    is_markup_enabled = table.Column<bool>(nullable: false),
                    company_id = table.Column<Guid>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateSheet", x => x.ratesheet_id);
                    table.ForeignKey(
                        name: "FK_RateSheet_Company_company_id",
                        column: x => x.company_id,
                        principalTable: "Company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RateSheet_company_id",
                table: "RateSheet",
                column: "company_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateSheet");
        }
    }
}
