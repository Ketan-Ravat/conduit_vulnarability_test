using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class irimagemapping_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IRScanWOImageFileMapping",
                columns: table => new
                {
                    irscanwoimagefilemapping_id = table.Column<Guid>(nullable: false),
                    img_file_name = table.Column<string>(nullable: true),
                    wo_id = table.Column<Guid>(nullable: false),
                    manual_wo_number = table.Column<string>(nullable: true),
                    is_img_attached = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: false),
                    created_by = table.Column<string>(nullable: true),
                    updated_at = table.Column<DateTime>(nullable: true),
                    updated_by = table.Column<string>(nullable: true),
                    is_deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IRScanWOImageFileMapping", x => x.irscanwoimagefilemapping_id);
                    table.ForeignKey(
                        name: "FK_IRScanWOImageFileMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IRScanWOImageFileMapping_wo_id",
                table: "IRScanWOImageFileMapping",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IRScanWOImageFileMapping");
        }
    }
}
