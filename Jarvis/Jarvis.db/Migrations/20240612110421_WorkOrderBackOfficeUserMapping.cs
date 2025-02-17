using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Jarvis.db.Migrations
{
    public partial class WorkOrderBackOfficeUserMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkOrderBackOfficeUserMapping",
                columns: table => new
                {
                    wo_backoffice_user_mapping_id = table.Column<Guid>(nullable: false),
                    wo_id = table.Column<Guid>(nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    site_id = table.Column<Guid>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    created_at = table.Column<DateTime>(nullable: true),
                    created_by = table.Column<string>(nullable: true),
                    modified_at = table.Column<DateTime>(nullable: true),
                    modified_by = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkOrderBackOfficeUserMapping", x => x.wo_backoffice_user_mapping_id);
                    table.ForeignKey(
                        name: "FK_WorkOrderBackOfficeUserMapping_Sites_site_id",
                        column: x => x.site_id,
                        principalTable: "Sites",
                        principalColumn: "site_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderBackOfficeUserMapping_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkOrderBackOfficeUserMapping_WorkOrders_wo_id",
                        column: x => x.wo_id,
                        principalTable: "WorkOrders",
                        principalColumn: "wo_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBackOfficeUserMapping_site_id",
                table: "WorkOrderBackOfficeUserMapping",
                column: "site_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBackOfficeUserMapping_user_id",
                table: "WorkOrderBackOfficeUserMapping",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkOrderBackOfficeUserMapping_wo_id",
                table: "WorkOrderBackOfficeUserMapping",
                column: "wo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkOrderBackOfficeUserMapping");
        }
    }
}
